using UnityEngine;

public class WhetStoneStarEffect : MonoBehaviour , IStarEffect
{
    [SerializeField] private float _damageBuffAmount = 1f;

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weaponToBuff = targetItem as WeaponBehaviour;
        weaponToBuff?.AddDamageToWeapon(_damageBuffAmount);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weaponToBuff = targetItem as WeaponBehaviour;
        weaponToBuff?.AddDamageToWeapon(-_damageBuffAmount);
    }

}
