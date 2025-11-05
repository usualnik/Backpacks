using System;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public event Action<ItemState, ItemState> OnItemStateChanged;

    public ItemState CurrentState => _currentState;
    public ItemState PreviousState => _previousState;
    public ItemDataSO ItemData => _itemData;

    [SerializeField] private ItemDataSO _itemData;


    [Flags]
    public enum ItemState
    {
        None = 0,                   // 0
        FreeFall = 1 << 0,          // 1
        Store = 1 << 1,             // 2
        Inventory = 1 << 2,         // 4
        Storage = 1 << 3,           // 8
        Dragging = 1 << 4,          // 16
    }

    [SerializeField] private ItemState _currentState;
    [SerializeField] private ItemState _previousState = ItemState.None;


    private int _itemPrice;
    private ItemVisual _itemVisual;

    public enum Target
    {
        Player,
        Enemy
    }

    [SerializeField] private Target _target;


    private void Awake()
    {
        //Эта строчка существет потому что цена объекта зафиксирована в SO и она readonly, из-за природы SO
        _itemPrice = _itemData.Price;
        _itemVisual = GetComponentInChildren<ItemVisual>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;

    }

    private void CombatManager_OnCombatStarted()
    {
        if (CurrentState.HasFlag(ItemState.Inventory))
        {
            _itemData.PerformAction(_target);
            //HACK: Применяется только первый эффект в списке - это неверно
            _itemData.Effects[0].ApplyEffect(_target);
        }       
    }

    public ItemState GetItemState() { return _currentState; }

    public void SetItemState(ItemState state)
    {
        //HACK: Я не уверен, что упраление состоянием работает правильно, нужно за нима следить
        if (_currentState != state)
        {
            _previousState = _currentState;

            _currentState = state;

            OnItemStateChanged?.Invoke(_previousState, _currentState);

        }
        else
        {
            _previousState = _currentState;
        }
    }

    public void InitItemStateInStore()
    {
        _previousState = ItemState.Store;
        _currentState = ItemState.Store;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void CombineWithIngridient(ItemBehaviour ingridient, ItemDataSO recipeResult)
    {
        _itemData = recipeResult;
        Destroy(ingridient.gameObject);
        _itemVisual.UpdateVisual(_itemData.Icon);
    }
    public int GetItemPrice() => _itemPrice;
    public void SetItemPrice(int value) => _itemPrice = value;
    public ItemVisual GetItemVisual() => _itemVisual;

}
