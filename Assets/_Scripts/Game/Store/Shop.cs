using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    public static Shop Instance { get; private set; }
    public event Action OnRerollPriceChanged;

    [SerializeField] private Transform[] _shopTransforms;
    [SerializeField] private List<GameObject> _items = new List<GameObject>();

    [Header("Prices")]
    [SerializeField] private TextMeshProUGUI[] _priceTexts;
    [SerializeField] private GameObject[] _salePricesObjects;

    [Header("SYSTEM")]
    [SerializeField] private ItemDataSO[] _allSpawnebleItems;

    private const float SALE_CHANCE = 0.2f;
    private const int MAX_SPAWN_ATTEMPTS = 50; // Защита от бесконечного цикла

    private int _rerollPrice = 1;
    private int _rerollProgression = 0;
    private const int REROLLS_NEEDED_TO_NEW_PRICE = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of shop manager");
    }

    private void Start()
    {
        SpawnItems();
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        foreach (var salePriceObj in _salePricesObjects)
        {
            salePriceObj.SetActive(false);
        }

        for (int i = 0; i < _shopTransforms.Length; i++)
        {
            // Проверяем, есть ли в слоте ЗАРЕЗЕРВИРОВАННЫЙ предмет
            bool hasReservedItem = false;
            ShopItem[] shopItemsInSlot = _shopTransforms[i].GetComponentsInChildren<ShopItem>();

            foreach (ShopItem shopItem in shopItemsInSlot)
            {
                if (shopItem.GetIsItemReserved())
                {
                    hasReservedItem = true;
                    break;
                }
            }

            // Если есть зарезервированный предмет - пропускаем этот слот
            if (hasReservedItem)
            {
                continue;
            }

            // Если в слоте есть незарезервированные предметы - очищаем их
            if (shopItemsInSlot.Length > 0)
            {
                foreach (ShopItem shopItem in shopItemsInSlot)
                {
                    if (!shopItem.GetIsItemReserved())
                    {
                        _items.Remove(shopItem.gameObject);
                        Destroy(shopItem.gameObject);
                    }
                }
            }

            // Спавним новый предмет с защитой от бесконечного цикла
            GameObject shopObject = SpawnRandomItemInSlot(i);
            if (shopObject != null)
            {
                _items.Add(shopObject);
            }
        }
    }

    private GameObject SpawnRandomItemInSlot(int slotIndex)
    {
        int attempts = 0;

        while (attempts < MAX_SPAWN_ATTEMPTS)
        {
            attempts++;

            int randomIndex = Random.Range(0, _allSpawnebleItems.Length);
            GameObject itemPrefab = _allSpawnebleItems[randomIndex].Prefab;

            if (itemPrefab == null)
            {
                Debug.LogWarning($"Item prefab is null for item: {_allSpawnebleItems[randomIndex].name}");
                continue;
            }

            ShopItem newShopItem = itemPrefab.GetComponent<ShopItem>();

            if (newShopItem == null)
            {
                Debug.LogWarning($"ShopItem component is missing on prefab: {itemPrefab.name}");
                continue;
            }

            bool canBeSpawnedInShop = _allSpawnebleItems[randomIndex].IsSpawnableInShop;

            if (canBeSpawnedInShop)
            {
                GameObject shopObject = Instantiate(itemPrefab, _shopTransforms[slotIndex].position, Quaternion.identity);
                shopObject.transform.SetParent(_shopTransforms[slotIndex], true);

                ItemBehaviour shopObjectItemBehaviour = shopObject.GetComponent<ItemBehaviour>();
                shopObjectItemBehaviour?.InitItemStateInStore();

                bool itemOnSale = Random.value < SALE_CHANCE;

                if (itemOnSale)
                {
                    int originalPrice = shopObjectItemBehaviour.GetItemPrice();
                    int salePrice = Mathf.RoundToInt(originalPrice / 2f);

                    // Price
                    _priceTexts[slotIndex].text = originalPrice.ToString();
                    // Sale price
                    _salePricesObjects[slotIndex].SetActive(true);
                    _salePricesObjects[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = salePrice.ToString();

                    shopObjectItemBehaviour.SetItemPrice(salePrice);
                }
                else
                {
                    // Sets a price for every spawned item
                    _priceTexts[slotIndex].text = shopObjectItemBehaviour.GetItemPrice().ToString();
                }

                return shopObject;
            }
        }

        Debug.LogError($"Failed to spawn item in slot {slotIndex} after {MAX_SPAWN_ATTEMPTS} attempts");
        return null;
    }

    public void Reroll()
    {
        _rerollProgression++;

        if (_rerollProgression % REROLLS_NEEDED_TO_NEW_PRICE == 0)
        {
            _rerollPrice++;
            OnRerollPriceChanged?.Invoke();
        }

        PlayerCharacter.Instance.SpendMoney(_rerollPrice);
        SpawnItems();
    }

    [ContextMenu("LOAD ALL ITEMS")]
    private void InitShopItems()
    {
        _allSpawnebleItems = Resources.LoadAll<ItemDataSO>("ItemsData")
            .Where(item => item.IsSpawnableInShop)
            .ToArray();

        Debug.Log($"Loaded {_allSpawnebleItems.Length} items");

        foreach (var itemData in _allSpawnebleItems)
        {
            if (itemData.Prefab == null)
            {
                Debug.LogError($"Item {itemData.name} has null prefab!");
                continue;
            }

            ShopItem shopItem = itemData.Prefab.GetComponent<ShopItem>();
            if (shopItem == null)
            {
                Debug.LogError($"Item {itemData.name} prefab is missing ShopItem component!");
            }
        }
    }

    public int GetCurrentRerollPrice() => _rerollPrice;
}