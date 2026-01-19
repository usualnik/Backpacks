using UnityEngine;

public class BagOfStonesStarEffect : MonoBehaviour, IStarEffect
{
    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour targetWeapon = targetItem as WeaponBehaviour;

        if (targetWeapon != null)
        {
            targetWeapon.IsCanAutoAttack(true);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour targetWeapon = targetItem as WeaponBehaviour;

        if (targetWeapon != null)
        {
            targetWeapon.IsCanAutoAttack(false);
        }

    }
}
