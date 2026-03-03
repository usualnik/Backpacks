using NUnit.Framework.Internal.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionBeltEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    private Bag _bag;
    private List<ItemBehaviour> _itemsInPotionBelt;
    private ItemBehaviour _potionBelt;

    private int _potionConsumedCounter = 0;

    private const int FIRST_POTION_CONSUMED = 1;
    private const int FOURTH_POTION_CONSUMED = 4;

    private void Awake()
    {
        _bag = GetComponent<Bag>();
        _potionBelt = GetComponent<ItemBehaviour>();
        _itemsInPotionBelt = new List<ItemBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_itemsInPotionBelt.Count > 0)
        {
            foreach (var i in _itemsInPotionBelt)
            {
                if (i.TryGetComponent(out IPotionEffect iPotionEffect))
                {
                    iPotionEffect.OnPotionConsumed -= IPotionEffect_OnPotionConsumed;
                }
            }
        }
       
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_itemsInPotionBelt.Count > 0)
        {
            foreach (var i in _itemsInPotionBelt)
            {
                if (i.TryGetComponent(out IPotionEffect iPotionEffect))
                {
                    iPotionEffect.OnPotionConsumed -= IPotionEffect_OnPotionConsumed;
                }
            }
        }

        _potionConsumedCounter = 0;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _itemsInPotionBelt = _bag.ItemsInbag;

        foreach (var i in _itemsInPotionBelt)
        {
            if (i.TryGetComponent(out IPotionEffect iPotionEffect))
            {
                iPotionEffect.OnPotionConsumed += IPotionEffect_OnPotionConsumed;
            }
        }
    }

    private void IPotionEffect_OnPotionConsumed()
    {
        _potionConsumedCounter++;

        if (_potionConsumedCounter == FIRST_POTION_CONSUMED)
        {
            FirstPotionConumed();
        }
        else if(_potionConsumedCounter == FOURTH_POTION_CONSUMED)
        {
            FourthPotionConsumed();
        }
    }
    private void FirstPotionConumed()
    {
        if(_potionBelt.OwnerCharacter ==  null) { return; }

        Buff randomBuff = new Buff
        {
            Name = "PotionBeltRandomBuff",
            Type = Buff.GetRandomBuffType(),
            IsPositive = true,
            Value = 1,

        };

        _potionBelt.OwnerCharacter.ApplyBuff(randomBuff);
    }
    private void FourthPotionConsumed()
    {
        if (_potionBelt.OwnerCharacter == null) { return; }

        Buff.BuffType randomDebuffTypeToRemove = _potionBelt.OwnerCharacter.AllDebuffs.FirstOrDefault().Type;
        _potionBelt.OwnerCharacter.RemoveBuff(randomDebuffTypeToRemove, 8);
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
