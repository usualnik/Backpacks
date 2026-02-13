using UnityEngine;

public class CrossBladesStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _damageBuff = 10f;
    [SerializeField] private float _speedIncreaseBuff = 0.6f;


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weapon = targetItem as WeaponBehaviour;

        if (weapon != null)
        {
            weapon.AddDamageToWeapon(_damageBuff);
            weapon.CooldownMultiplier += _speedIncreaseBuff;
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        WeaponBehaviour weapon = targetItem as WeaponBehaviour;

        if (weapon != null)
        {
            weapon.AddDamageToWeapon(-_damageBuff);
            weapon.CooldownMultiplier -= _speedIncreaseBuff;
        }
    }
}
