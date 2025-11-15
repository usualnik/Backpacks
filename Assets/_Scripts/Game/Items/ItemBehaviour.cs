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
    public Character SourceCharacter => _sourceCharacter;

    [SerializeField] private ItemDataSO _itemData;

    private IItemEffect _effect;

    protected Character _targetCharacter;
    protected Character _sourceCharacter;


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

    public enum Target
    {
        Player,
        Enemy
    }

    [SerializeField] protected Target _target;


    private void Awake()
    {
        //Эта строчка существет потому что цена объекта зафиксирована в SO и она readonly, из-за природы SO
        _itemPrice = _itemData.Price;
        _itemVisual = GetComponentInChildren<ItemVisual>();
        _effect = GetComponent<IItemEffect>();        
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
            switch (GetTarget())
            {
                case Target.Player:
                    _targetCharacter = PlayerCharacter.Instance;
                    _sourceCharacter = EnemyCharacter.Instance;
                    break;
                case Target.Enemy:
                    _targetCharacter = EnemyCharacter.Instance;
                    _sourceCharacter = PlayerCharacter.Instance;
                    break;
            }

            //_itemData.PerformAction(_target,this);

            PerformAction();

            OnItemActionPerformed?.Invoke(this, _targetCharacter);
            
            _effect?.ApplyEffect(this,_sourceCharacter,_targetCharacter);
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
    public ItemBehaviour.Target GetTarget() => _target;

    private void PerformAction()
    {

    }
}
