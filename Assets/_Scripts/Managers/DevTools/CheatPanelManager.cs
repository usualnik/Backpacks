using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CheatPanelManager : MonoBehaviour
{
    public static CheatPanelManager Instance { get; private set; }
    [SerializeField] private List<ItemBehaviour> _allItemsInGame;

    [Header("System")]
    [SerializeField] private GameObject _cheatPanel;
    [SerializeField] private TMP_InputField _searchInputField;

    [SerializeField] private ItemBehaviour _trackedItem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one instance of cheatPanel manager");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _searchInputField.onValueChanged.AddListener(SearchItems);
        StartCombatButton.OnStartCombatButtonPressed += StartCombatButton_OnStartCombatButtonPressed;

        if (SelectedItemManager.Instance != null)
            SelectedItemManager.Instance.OnItemSelected += SelectedItemManager_OnItemSelected;

        InitCheatPanel();
    }

    private void OnDestroy()
    {
        _searchInputField.onValueChanged.RemoveListener(SearchItems);
        StartCombatButton.OnStartCombatButtonPressed -= StartCombatButton_OnStartCombatButtonPressed;

        if (SelectedItemManager.Instance != null)
            SelectedItemManager.Instance.OnItemSelected -= SelectedItemManager_OnItemSelected;

        if (_trackedItem != null)
        {
            _trackedItem.OnItemStateChanged -= TrackedItem_OnItemStateChanged;
            _trackedItem = null;
        }
    }

    private void SelectedItemManager_OnItemSelected(ItemBehaviour obj)
    {
        // Отписываемся от предыдущего предмета
        if (_trackedItem != null)
        {
            _trackedItem.OnItemStateChanged -= TrackedItem_OnItemStateChanged;
            _trackedItem = null;
        }

        // Подписываемся на новый, если он подходит
        if (obj != null && obj.TryGetComponent(out CheatPanelItem cheatPanelItem))
        {
            _trackedItem = obj;
            _trackedItem.OnItemStateChanged += TrackedItem_OnItemStateChanged;
        }
    }

    private void TrackedItem_OnItemStateChanged(ItemBehaviour.ItemState arg1, ItemBehaviour.ItemState arg2)
    {
        if (arg2 == ItemBehaviour.ItemState.Inventory)
        {
            _trackedItem.enabled = true;

            // Отписываемся после перемещения в инвентарь
            _trackedItem.OnItemStateChanged -= TrackedItem_OnItemStateChanged;
            _trackedItem = null;
        }
    }

    private void StartCombatButton_OnStartCombatButtonPressed()
    {
        if (_cheatPanel != null)
            _cheatPanel.SetActive(false);
    }

    private void InitCheatPanel()
    {
        // Очищаем список от уничтоженных объектов
        _allItemsInGame = _allItemsInGame.Where(item => item != null).ToList();

        foreach (var item in _allItemsInGame)
        {
            if (item != null)
            {
                item.InitItemStateInStore();
                item.enabled = false;
            }
        }
    }

    private void SearchItems(string searchText)
    {
        // Очищаем список от уничтоженных объектов при поиске
        _allItemsInGame.RemoveAll(item => item == null);

        if (string.IsNullOrEmpty(searchText))
        {
            foreach (var item in _allItemsInGame)
            {
                if (item != null)
                    item.gameObject.SetActive(true);
            }
            return;
        }

        string lowerSearchText = searchText.ToLower();

        foreach (var item in _allItemsInGame)
        {
            if (item != null && item.ItemData != null)
            {
                bool isMatch = item.ItemData.ItemName.ToLower().Contains(lowerSearchText);
                item.gameObject.SetActive(isMatch);
            }
        }
    }

    public void ToggleCheatPanel()
    {
        if (_cheatPanel != null)
            _cheatPanel.SetActive(!_cheatPanel.activeInHierarchy);
    }
}