using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Items/Melee Weapon Data")]
public class MeleeWeaponDataSO : WeaponDataSO
{ 
    public override void PerformAction(ItemBehaviour.Target target, ItemBehaviour performedItem)
    {
               
    }

    public override void PerformWeaponAction(ItemBehaviour.Target target, WeaponBehaviour performedItem)
    {
        CombatManager.Instance.StartAutoAttack(target, performedItem,
            performedItem.WeaponDamageMin, performedItem.WeaponDamageMax,
            _staminaCost, _cooldown, _accuracy);

    }


}
