using System.Collections.Generic;
using UnityEngine;

public class RangerBagEffect : MonoBehaviour, IItemEffect
{
    private Bag _rangerBag;

    [SerializeField]
    private List<WeaponBehaviour> _weaponsToBuff;
    private ItemBehaviour _itemBehaviour;
    private Character _targetCharacter;

    private float _critHitChanceBuff = 10f;
    private const float CRIT_HIT_CHANCE_PER_LUCK_AMOUNT = 3f;



    private void Awake()
    {
        _rangerBag = GetComponent<Bag>();
    }

    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
        _itemBehaviour = target;


        foreach (var item in _rangerBag.ItemsInbag)
        {
            if (item is WeaponBehaviour)
            {
                _weaponsToBuff.Add(item as WeaponBehaviour);               
            }
        }

        foreach (var weapon in _weaponsToBuff)
        {
            weapon.AddCritHitChanceToWeapon(CalculateFinalCritHitChance());
        }
    }

    public void RemoveEffect()
    {
    }

    private float CalculateFinalCritHitChance()
    {
        float finalCritChance = 0f;
        
        if (_itemBehaviour.GetTarget() == ItemBehaviour.Target.Player)
        {
            _targetCharacter = PlayerCharacter.Instance;
        }
        else if(_itemBehaviour.GetTarget() == ItemBehaviour.Target.Enemy)
        {
            _targetCharacter = EnemyCharacter.Instance;
        }

        finalCritChance = _critHitChanceBuff + (_targetCharacter.GetLuckStacks() * CRIT_HIT_CHANCE_PER_LUCK_AMOUNT);

        return finalCritChance;
    }
}
