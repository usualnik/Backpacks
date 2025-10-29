using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop Instance { get; private set; }

    [SerializeField] private Transform[] _shopTransforms;
    [SerializeField] private List<GameObject> _items = new List<GameObject>();

    [Header("Prices")]
    [SerializeField] private TextMeshProUGUI[] _priceTexts;
    [SerializeField] private GameObject[] _salePricesObjects;


    [Header("SYSTEM")]
    [SerializeField] private ItemDataSO[] _allSpawnebleItems;

    private const float SALE_CHANCE = 0.2f;


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
            int randomIndex = Random.Range(0, _allSpawnebleItems.Length);

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

    }


    public void Reroll()
    {
        ClearShop();
        SpawnItems();
    }

    private void ClearShop()
    {
        foreach (var item in _items)
        {
            Destroy(item);
        }
    }

    [ContextMenu("LOAD ALL ITEMS")]
    private void InitShopItems()
    {
        _allSpawnebleItems = Resources.LoadAll<ItemDataSO>("ItemsData");
    }


}
