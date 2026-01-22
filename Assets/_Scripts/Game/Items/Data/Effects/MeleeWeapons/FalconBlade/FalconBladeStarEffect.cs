using System.Collections.Generic;
using UnityEngine;

public class FalconBladeStarEffect : MonoBehaviour, IStarEffect
{

    [SerializeField] private List<ItemBehaviour> _starredWeapons = new List<ItemBehaviour>();
    [SerializeField] private float _cooldownIncrease = 0.3f;

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
        if (_starredWeapons.Count > 0)
        {
            CooldownBuff();
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredWeapons.Contains(targetItem) && targetItem is WeaponBehaviour)
        {
            _starredWeapons.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_starredWeapons.Contains(targetItem) && targetItem is WeaponBehaviour)
        {
            _starredWeapons.Remove(targetItem);
        }
    }

    private void CooldownBuff()
    {
        foreach (var item in _starredWeapons)
        {
            item.TryGetComponent(out WeaponBehaviour weapon);

            if (weapon != null)
            {
                weapon.IncreaseSpeedMultiplier(_cooldownIncrease);
            }
        }
    }


}
