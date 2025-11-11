using UnityEngine;

[CreateAssetMenu(fileName = "WhetStoneStarEffectSO", menuName = "Items/StarEffects/Accessory/WhetStoneStarEffect")]
public class WhetStoneStarEffectSO : StarEffectSO
{    
    public override void ApplyStarEffect(ItemBehaviour i)
    {
        WeaponBehaviour weaponBehaviour = i as WeaponBehaviour;
        
        if (weaponBehaviour != null)
        {
            weaponBehaviour.AddDamageToWeapon(Amount);
        }
    }

    public override void RemoveStarEffect(ItemBehaviour i)
    {
        WeaponBehaviour weaponBehaviour = i as WeaponBehaviour;
        
        if (weaponBehaviour != null)
        {
            weaponBehaviour.AddDamageToWeapon(-Amount);
        }

    }
}
