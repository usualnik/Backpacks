using System.Collections.Generic;
using UnityEngine;

public class VillainSwordStarEffect : MonoBehaviour, IStarEffect
{

    [SerializeField] private List<WeaponBehaviour> _starredMeleeWeapons = new List<WeaponBehaviour>();

    [SerializeField] private float _damageDebuffForStarredMeleeWeapon = -2f;
    [SerializeField] private float _damageBuffPerStarredMeleeWeapon = 4f;

    private WeaponBehaviour _villainSword;

    private void Awake()
    {
        _villainSword = GetComponent<WeaponBehaviour>();
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
        if (_starredMeleeWeapons.Count > 0)
        {
            ApplyBuff();
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour meleeWeapon = targetItem as WeaponBehaviour;

        if (meleeWeapon != null && meleeWeapon.ItemData.ItemExtraType == ItemDataSO.ExtraType.Melee)
        {

            if (!_starredMeleeWeapons.Contains(meleeWeapon))
            {
                _starredMeleeWeapons.Add(meleeWeapon);
            }
        }

    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour meleeWeapon = targetItem as WeaponBehaviour;

        if (meleeWeapon != null && meleeWeapon.ItemData.ItemExtraType == ItemDataSO.ExtraType.Melee)
        {

            if (_starredMeleeWeapons.Contains(meleeWeapon))
            {
                _starredMeleeWeapons.Remove(meleeWeapon);
            }
        }

    }


    private void ApplyBuff()
    {
        foreach (var weapon in _starredMeleeWeapons)
        {
            weapon.AddDamageToWeapon(_damageDebuffForStarredMeleeWeapon);
        }

        float finalDamageBuff = _starredMeleeWeapons.Count * _damageBuffPerStarredMeleeWeapon;

        _villainSword.AddDamageToWeapon(finalDamageBuff);
    }
}
