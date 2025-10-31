using System;
using System.Collections.Generic;
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

            // Спавним новый предмет
            int randomIndex = Random.Range(0, _allSpawnebleItems.Length);
            ShopItem newShopItem = _allSpawnebleItems[randomIndex].Prefab.GetComponent<ShopItem>();
            bool canBeSpawnedInShop = newShopItem.GetCanBeSawnedInShop();

            if (canBeSpawnedInShop)
            {
                GameObject shopObject = Instantiate(_allSpawnebleItems[randomIndex].Prefab, _shopTransforms[i].position, Quaternion.identity);
                shopObject.transform.SetParent(_shopTransforms[i], true);

                ItemBehaviour shopObjectItemBehaviour = shopObject.GetComponent<ItemBehaviour>();
                shopObjectItemBehaviour?.InitItemStateInStore();

                bool itemOnSale = Random.value < SALE_CHANCE;

                if (itemOnSale)
                {
                    int salePrice = Mathf.RoundToInt(shopObjectItemBehaviour.GetItemPrice() / 2f);

                    //Price
                    _priceTexts[i].text = shopObjectItemBehaviour.GetItemPrice().ToString();
                    //Sale price
                    _salePricesObjects[i].SetActive(true);
                    _salePricesObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = salePrice.ToString();

                    shopObjectItemBehaviour.SetItemPrice(salePrice);
                }
                else
                {
                    //Sets a price for every spawned item
                    _priceTexts[i].text = shopObjectItemBehaviour.GetItemPrice().ToString();
                }

                _items.Add(shopObject);
            }
            else
            {
                //Вычитаем 1 из итератора, чтобы повторить проход для этого слота
                i--;
            }
        }
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

        // Убираем ClearShop() и делаем всю логику в SpawnItems()
        SpawnItems();
    }

    // Убираем метод ClearShop() полностью, так как вся очистка теперь в SpawnItems()

    [ContextMenu("LOAD ALL ITEMS")]
    private void InitShopItems()
    {
        _allSpawnebleItems = Resources.LoadAll<ItemDataSO>("ItemsData");
    }

    public int GetCurrentRerollPrice() => _rerollPrice;
}