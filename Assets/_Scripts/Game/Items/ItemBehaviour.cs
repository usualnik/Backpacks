using System;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public event Action<ItemState, ItemState> OnItemStateChanged;

    public event Action<ItemBehaviour, Character> OnItemActionPerformed;
    public ItemState CurrentState => _currentState;
    public ItemState PreviousState => _previousState;
    public ItemDataSO ItemData => _itemData;
    public Character TargetCharacter => _targetCharacter;
    public Character OwnerCharacter => _ownerCharacter;

    [SerializeField] private ItemDataSO _itemData;

    private IItemEffect _effect;

    [SerializeField]
    protected Character _targetCharacter;
    [SerializeField]
    protected Character _ownerCharacter;


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

    [SerializeField] protected ItemState _currentState;
    [SerializeField] protected ItemState _previousState = ItemState.None;


    protected int _itemPrice;
    protected ItemVisual _itemVisual;

   protected OwnerTargetHandler _ownerTargetHandler;

    private void Awake()
    {
        _itemPrice = _itemData.Price;

        _ownerTargetHandler = GetComponent<OwnerTargetHandler>();
        _itemVisual = GetComponentInChildren<ItemVisual>();
        _effect = GetComponent<IItemEffect>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;

        ConfigureItemOwnerTarget();
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
    }
    private void CombatManager_OnCombatStarted()
    {
        if (CurrentState.HasFlag(ItemState.Inventory))
        {


            //_itemData.PerformAction(_target,this);

            PerformAction();

            OnItemActionPerformed?.Invoke(this, _targetCharacter);

            _effect?.ApplyEffect(this, _ownerCharacter, _targetCharacter);
        }
    }
    public ItemState GetItemState() { return _currentState; }
    public void SetItemState(ItemState state)
    {
        //HACK: Я не уверен, что упраление состоянием работает правильно, нужно за ним следить
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
    public void InitItemStateInInventory()
    {
        _previousState = ItemState.Inventory;
        _currentState = ItemState.Inventory;
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
    public int GetItemPrice() => ItemData.Price;
    public void SetItemPrice(int value) => _itemPrice = value;
    public ItemVisual GetItemVisual() => _itemVisual;
    public OwnerTargetHandler.Target GetTarget() => _ownerTargetHandler.GetTarget();
    public OwnerTargetHandler.Owner GetOwner() => _ownerTargetHandler.GetOwner();

    private void PerformAction()
    {

    }

    protected void ConfigureItemOwnerTarget()
    {
        switch (GetTarget())
        {
            case OwnerTargetHandler.Target.Player:
                _targetCharacter = PlayerCharacter.Instance;
                break;
            case OwnerTargetHandler.Target.Enemy:
                _targetCharacter = EnemyCharacter.Instance;
                break;
            case OwnerTargetHandler.Target.None:
                // У некоторых предметов нет цели, по дефолту ставится в цель игрок
                _targetCharacter = PlayerCharacter.Instance;
                break;
        }

        switch (GetOwner())
        {
            case OwnerTargetHandler.Owner.Player:
                _ownerCharacter = PlayerCharacter.Instance;
                break;
            case OwnerTargetHandler.Owner.Enemy:
                _ownerCharacter = EnemyCharacter.Instance;
                break;
        }
    }

}
