using System.Collections.Generic;
using UnityEngine;

public class HeroSwordStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<WeaponBehaviour> _trackedWeapons = new List<WeaponBehaviour>();

    [SerializeField] private float _damageBuff = 1f;

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
        if (_trackedWeapons.Count > 0)
        {
            BuffStarredWeapons();
        }
    }
    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weapon = targetItem as WeaponBehaviour;
        if (!_trackedWeapons.Contains(weapon))
        {
            _trackedWeapons.Add(weapon);
        }
    }
    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weapon = targetItem as WeaponBehaviour;
        if (_trackedWeapons.Contains(weapon))
        {
            _trackedWeapons.Remove(weapon);
        }
    }
    private void BuffStarredWeapons()
    {
        foreach (var weapon in _trackedWeapons)
        {
            weapon.AddDamageToWeapon(_damageBuff);
        }
    }

   
}
