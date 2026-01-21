using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EggscaliburStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _starredFood = new List<ItemBehaviour>();

    [SerializeField] private float _damagePerFoodBuff = 1f;

    private WeaponBehaviour _eggscalibur;

    private const int MANA_STACKS_NEEDED_TO_PROC = 11;

    private void Awake()
    {
        _eggscalibur = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _eggscalibur.ResetWeaponStatsToDefault();
    }


    private void CombatManager_OnCombatStarted()
    {
        if (_starredFood.Count > 0)
        {
            BuffDamagePerFood();
        }
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_eggscalibur != weapon) return;
        if (!_eggscalibur.OwnerCharacter) return;
       
        if (_starredFood.Count > 0 
            && _eggscalibur.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) > MANA_STACKS_NEEDED_TO_PROC)
        {
            TriggerAllFoodPerHit();
        }

    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredFood.Contains(targetItem))
        {
            _starredFood.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_starredFood.Contains(targetItem))
        {
            _starredFood.Remove(targetItem);
        }
    }

    private void BuffDamagePerFood()
    {
        _eggscalibur.AddDamageToWeapon(_damagePerFoodBuff * _starredFood.Count);
    }

    private void TriggerAllFoodPerHit()
    {
        _eggscalibur.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, MANA_STACKS_NEEDED_TO_PROC);

        foreach (var item in _starredFood)
        {
            item.TryGetComponent(out IFoodEffect foodEffect);

            if (foodEffect != null)
            {
                foodEffect.TriggerEffect();
            }
        }
    }

}
