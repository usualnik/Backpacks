using System.Collections.Generic;
using UnityEngine;

public class PiggyBankStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _trackedItems = new List<ItemBehaviour>();
    [SerializeField] private float _maxHealtPerItemBonus = 2f;

    private ItemBehaviour _piggyBank;

    private void Awake()
    {
        _trackedItems = new List<ItemBehaviour>();
        _piggyBank = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;


    }
    private void CombatManager_OnCombatStarted()
    {
        if(_piggyBank == null || _piggyBank.OwnerCharacter == null) { return; }

        float maxHealthBonus = _maxHealtPerItemBonus * _trackedItems.Count;

        _piggyBank.OwnerCharacter.ChangeMaxHealthValue(maxHealthBonus);

    }


    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_piggyBank == null || _piggyBank.OwnerCharacter == null) { return; }

        float maxHealthBonus = _maxHealtPerItemBonus * _trackedItems.Count;

        _piggyBank.OwnerCharacter.ChangeMaxHealthValue(-maxHealthBonus);


    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

        if (!_trackedItems.Contains(targetItem))
        {
            _trackedItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_trackedItems.Contains(targetItem))
        {
            _trackedItems.Remove(targetItem);
        }
    }

}
