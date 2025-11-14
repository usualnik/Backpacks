using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Items/Ranged Weapon Data")]

public class RangedWeaponDataSO : WeaponDataSO
{
    public override void PerformAction(ItemBehaviour.Target target, ItemBehaviour performedItem)
    {
        // AUTOATTACK
    }

    public override void PerformWeaponAction(ItemBehaviour.Target target, WeaponBehaviour performedItem)
    {
        CombatManager.Instance.StartAutoAttack(target, performedItem,
            performedItem.WeaponDamageMin, performedItem.WeaponDamageMax,
            _staminaCost, _cooldown, _accuracy);

    }

}
