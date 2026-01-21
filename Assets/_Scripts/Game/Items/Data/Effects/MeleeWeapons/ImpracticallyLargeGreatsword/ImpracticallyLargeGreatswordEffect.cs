using System;
using UnityEngine;

public class ImpracticallyLargeGreatswordEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    private float _cooldownBuff = 0.4f;
    private float _decreaseStaminaUsage = 3f;

    private WeaponBehaviour _greatSword;

    private const int EMPOWER_NEEDED_TO_PROC = 5;
    private bool _isEffectActive = false;

    private void Awake()
    {
        _greatSword = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _greatSword.OwnerCharacter.OnNewBuffApplied -= OwnerCharacter_OnNewBuffApplied;
        _greatSword.OwnerCharacter.OnBuffRemoved -= OwnerCharacter_OnBuffRemoved;

        _greatSword.ResetWeaponStatsToDefault();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _greatSword.OwnerCharacter.OnNewBuffApplied += OwnerCharacter_OnNewBuffApplied;
        _greatSword.OwnerCharacter.OnBuffRemoved += OwnerCharacter_OnBuffRemoved;
    }

    private void OwnerCharacter_OnBuffRemoved(Buff buff)
    {
        if (buff.Type == Buff.BuffType.Empower)
        {
            if (_greatSword.OwnerCharacter.GetBuffStacks(Buff.BuffType.Empower) < EMPOWER_NEEDED_TO_PROC && _isEffectActive)
            {
                RemoveEffect();
                _isEffectActive = false;
            }
        }
    }

    private void OwnerCharacter_OnNewBuffApplied(Buff buff)
    {
        if (buff.Type == Buff.BuffType.Empower)
        {
            if (_greatSword.OwnerCharacter.GetBuffStacks(Buff.BuffType.Empower) >= EMPOWER_NEEDED_TO_PROC && !_isEffectActive)
            {
                ProcEffect();
                _isEffectActive = true;
            }
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void ProcEffect()
    {
        _greatSword.IncreaseSpeedMultiplier(_cooldownBuff);
        _greatSword.RemoveStaminaUsage(_decreaseStaminaUsage);
    }

    private void RemoveEffect()
    {

    }
   

}
