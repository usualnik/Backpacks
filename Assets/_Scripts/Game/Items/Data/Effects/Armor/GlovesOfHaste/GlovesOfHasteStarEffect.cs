using System.Collections.Generic;
using UnityEngine;

public class GlovesOfHasteStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _speedIncrease = 0.2f;

    private List<ItemBehaviour> _itemsInStar = new List<ItemBehaviour>();


    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
    }   

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Remove(targetItem);
        }
    }

    private void CombatManager_OnCombatStarted()
    {
        foreach(ItemBehaviour sourceItem in _itemsInStar)
        {
            sourceItem.TryGetComponent(out WeaponBehaviour weapon);
            if (weapon != null)
            {
                weapon.CooldownMultiplier += _speedIncrease;
            }
        }
    }
}
