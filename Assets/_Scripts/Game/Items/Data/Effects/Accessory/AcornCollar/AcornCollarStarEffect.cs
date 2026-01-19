using UnityEngine;

public class AcornCollarStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _critHitIncrese = 5f;

    private Character _owner;

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _owner = sourceItem.OwnerCharacter;

        WeaponBehaviour weapon = targetItem as WeaponBehaviour;

        if (weapon != null)
        {
            int luckStacks = _owner.GetBuffStacks(Buff.BuffType.Luck);

            IncreaseCritHitChancePerLuckStack(weapon,_critHitIncrese, luckStacks);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _owner = sourceItem.OwnerCharacter;

        WeaponBehaviour weapon = targetItem as WeaponBehaviour;

        if (weapon != null)
        {
            int luckStacks = _owner.GetBuffStacks(Buff.BuffType.Luck);

            IncreaseCritHitChancePerLuckStack(weapon, -_critHitIncrese, luckStacks);
        }
    }


    private void IncreaseCritHitChancePerLuckStack(WeaponBehaviour weapon, float critHitChance, int luckStacks)
    {
        for (int i = 0; i < luckStacks; i++)
        {
            weapon.AddCritHitChanceToWeapon(critHitChance);
        }
    }
}
