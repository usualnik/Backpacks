using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop Instance { get; private set; }

    [SerializeField] private Transform[] _shopTransforms;
    [SerializeField] private TextMeshProUGUI[] _priceTexts;
    [SerializeField] private List<GameObject> _items = new List<GameObject>();


    [Header("SYSTEM")]
    [SerializeField] private ItemDataSO[] _allSpawnebleItems;

  

    
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
       
        for (int i = 0; i < _shopTransforms.Length; i++)
        {
            int randomIndex = Random.Range(0, _allSpawnebleItems.Length);

            GameObject shopItem = Instantiate(_allSpawnebleItems[randomIndex].Prefab, _shopTransforms[i].position, Quaternion.identity);
            shopItem.transform.SetParent(_shopTransforms[i], true);
            
            //Sets a price for every spawned item
            _priceTexts[i].text = _allSpawnebleItems[randomIndex].Price.ToString();
            
            _items.Add(shopItem);
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
