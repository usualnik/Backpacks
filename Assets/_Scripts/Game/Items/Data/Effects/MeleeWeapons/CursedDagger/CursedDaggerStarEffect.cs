using System.Collections.Generic;
using UnityEngine;

public class CursedDaggerStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _starredItems = new List<ItemBehaviour>();
    [SerializeField] private float _accuracyBuff = 1f;
    [SerializeField] private float _critHitChanceBuff = 1f;

    private WeaponBehaviour _cursedDagger;

    private int _opponentDebuffs = 0;

    private void Awake()
    {
        _cursedDagger = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        _cursedDagger.TargetCharacter.OnNewBuffApplied += TargetCharacter_OnNewBuffApplied;
        _cursedDagger.TargetCharacter.OnBuffRemoved += TargetCharacter_OnBuffRemoved;
    }   

    private void OnDestroy()
    {
        _cursedDagger.TargetCharacter.OnNewBuffApplied -= TargetCharacter_OnNewBuffApplied;
        _cursedDagger.TargetCharacter.OnBuffRemoved -= TargetCharacter_OnBuffRemoved;


    }
    private void TargetCharacter_OnNewBuffApplied(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Cold 
            || newBuff.Type == Buff.BuffType.Poison 
            || newBuff.Type == Buff.BuffType.Blindness)
        {
            _opponentDebuffs++;
            RecalculateStats();
        }
    }

    private void TargetCharacter_OnBuffRemoved(Buff removedBuff)
    {
        if (removedBuff.Type == Buff.BuffType.Cold
           || removedBuff.Type == Buff.BuffType.Poison
           || removedBuff.Type == Buff.BuffType.Blindness)
        {
            _opponentDebuffs--;
            RecalculateStats();
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredItems.Contains(targetItem))
        {
            _starredItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_starredItems.Contains(targetItem))
        {
            _starredItems.Remove(targetItem);
        }
    }

    private void RecalculateStats()
    {
        _cursedDagger.AddAccuracyToWeapon(_accuracyBuff * _opponentDebuffs);
        _cursedDagger.AddCritHitChanceToWeapon(_critHitChanceBuff * _opponentDebuffs);

        foreach (var item in _starredItems)
        {
            if (item is WeaponBehaviour)
            {
                WeaponBehaviour weapon = item as WeaponBehaviour;

                weapon.AddAccuracyToWeapon(_accuracyBuff * _opponentDebuffs);
                weapon.AddCritHitChanceToWeapon(_critHitChanceBuff * _opponentDebuffs);
            }
        }

    }
}
