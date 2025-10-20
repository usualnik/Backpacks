using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class ItemBehaviour : MonoBehaviour
{    
    public event Action<ItemState, ItemState> OnItemStateChanged;

    public ItemDataSO ItemData => _itemData;

    [SerializeField] private ItemDataSO _itemData;

    public enum ItemState
    {
        None,
        FreeFall,      
        Store,
        Inventory,
        Storage
    }

    [SerializeField] private ItemState _currentState;
    [SerializeField] private ItemState _previousState = ItemState.None; 



    public enum Target
    {
        Player,
        Enemy
    }

    [SerializeField] private Target _target;

    public ItemState CurrentState => _currentState;
    public ItemState PreviousState => _previousState;

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
        _itemData.PerformAction(_target);

        //Stub
        _itemData.Effects[0].ApplyEffect(_target);
    }    

    public ItemState GetItemState() { return _currentState; }
   
    public void SetItemState(ItemState state)
    {
        if (_currentState != state)
        {
            _previousState = _currentState;

            _currentState = state;

            OnItemStateChanged?.Invoke(_previousState, _currentState);
            
        }





    }
}
