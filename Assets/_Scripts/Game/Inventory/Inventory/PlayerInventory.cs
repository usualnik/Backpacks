using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : BaseInventory
{
    public static PlayerInventory Instance {  get; private set; }

    [SerializeField] private GameObject _rangerStartItemsConfig;
    [SerializeField] private GameObject _reaperStartItemsConfig;

    private List<GameObject> _configs = new List<GameObject>();

    private ItemBehaviour _currentSelectedItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than one instance of Player Inventory");
        }

        _configs.Add(_rangerStartItemsConfig);
        _configs.Add(_reaperStartItemsConfig);
    }
    private void Start()
    {
        //InitStartItems();
        PlayerCharacter.Instance.OnPlayerClassChanged += PlayerCharacter_OnPlayerClassChanged;
        SelectedItemManager.Instance.OnItemSelected += SelectedItemManager_OnItemSelected;
    }

    private void OnDestroy()
    {
        PlayerCharacter.Instance.OnPlayerClassChanged -= PlayerCharacter_OnPlayerClassChanged;
        SelectedItemManager.Instance.OnItemSelected -= SelectedItemManager_OnItemSelected;
    }

    private void SelectedItemManager_OnItemSelected(ItemBehaviour newItem)
    {               
        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.OnItemStateChanged -= SelectedItem_OnItemStateChanged;
        }

        _currentSelectedItem = newItem;

        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.OnItemStateChanged += SelectedItem_OnItemStateChanged;

            SelectedItem_OnItemStateChanged(_currentSelectedItem.PreviousState, _currentSelectedItem.CurrentState);

        }
    }
    private void SelectedItem_OnItemStateChanged(ItemBehaviour.ItemState previousState, ItemBehaviour.ItemState newState)
    {
        if (previousState.HasFlag(ItemBehaviour.ItemState.Inventory)
            && !newState.HasFlag(ItemBehaviour.ItemState.Inventory))
        {
            RemoveItemFromInventory(_currentSelectedItem);
        }

        if (newState.HasFlag(ItemBehaviour.ItemState.Inventory) 
            && !previousState.HasFlag(ItemBehaviour.ItemState.Inventory))
        {
            AddItemToInventory(_currentSelectedItem);
        }
    }

    private void PlayerCharacter_OnPlayerClassChanged(ClassDataSO newPlayerClass)
    {
        UpdateItemsConfig(newPlayerClass.Class);
    }

    private void InitStartItems()
    {
        _itemsInIventory.Clear();

        switch (PlayerCharacter.Instance.ClassData.Class)
        {
            case ClassDataSO.ClassType.Ranger:

                _rangerStartItemsConfig.gameObject.SetActive(true);

                _itemsInIventory.AddRange(PlayerCharacter.Instance.ClassData.GetAllStartItems());
                break;

            case ClassDataSO.ClassType.Reaper:

                _reaperStartItemsConfig.gameObject.SetActive(true);

                _itemsInIventory.AddRange(PlayerCharacter.Instance.ClassData.GetAllStartItems());
                break;
            default:
                break;
        }
    }
    private void UpdateItemsConfig(ClassDataSO.ClassType newPlayerClass)
    {
        _itemsInIventory.Clear();

        foreach (var item in _configs)
        {
            item.gameObject.SetActive(false);
        }

        switch (newPlayerClass)
        {
            case ClassDataSO.ClassType.Ranger:
                _rangerStartItemsConfig.SetActive(true);

                _itemsInIventory.AddRange(PlayerCharacter.Instance.ClassData.GetAllStartItems());

                break;
            case ClassDataSO.ClassType.Reaper:
                _reaperStartItemsConfig.SetActive(true);

                _itemsInIventory.AddRange(PlayerCharacter.Instance.ClassData.GetAllStartItems());

                break;
            default:
                break;
        }
    }

    private void AddItemToInventory(ItemBehaviour item)
    {
        if (!_itemsInIventory.Contains(item))
        {
            _itemsInIventory.Add(item);
        }
    }

    private void RemoveItemFromInventory(ItemBehaviour item)
    {
        if (_itemsInIventory.Contains(item))
        {
            _itemsInIventory.Remove(item);
        }
    }

    public float GetPlayerGearScore()
    {
        float gearscore = 0f;

        foreach (var item in _itemsInIventory)
        {
            gearscore += item.ItemData.GearScore;
        }

        return gearscore;
    }
}
