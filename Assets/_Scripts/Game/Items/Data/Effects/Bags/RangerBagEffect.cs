using System.Collections.Generic;
using UnityEngine;

public class RangerBagEffect : MonoBehaviour, IItemEffect
{
    private Bag _rangerBag;

    private List<WeaponBehaviour> _weaponsToBuff = new List<WeaponBehaviour>();
    private ItemBehaviour _itemBehaviour;
    private Character _targetCharacter;

    private float _critHitChanceBuff = 10f;
    private const float CRIT_HIT_CHANCE_PER_LUCK_AMOUNT = 3f;



    private void Awake()
    {
        _rangerBag = GetComponent<Bag>();
    }

    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        _itemBehaviour = item;
        _targetCharacter = targetCharacter;

        if (_rangerBag.ItemsInbag.Count > 0)
        {
            foreach (var i in _rangerBag.ItemsInbag)
            {
                if (i is WeaponBehaviour)
                {
                    _weaponsToBuff.Add(i as WeaponBehaviour);
                }
            }
        }

        if (_weaponsToBuff.Count > 0)
        {
            foreach (var weapon in _weaponsToBuff)
            {
                weapon?.AddCritHitChanceToWeapon(CalculateFinalCritHitChance());
            }
        }
       
    }

    public void RemoveEffect()
    {
    }

    private float CalculateFinalCritHitChance()
    {
        float finalCritChance = 0f;       
       
        finalCritChance = _critHitChanceBuff + (_targetCharacter.GetLuckStacks() * CRIT_HIT_CHANCE_PER_LUCK_AMOUNT);

        return finalCritChance;
    }
}
