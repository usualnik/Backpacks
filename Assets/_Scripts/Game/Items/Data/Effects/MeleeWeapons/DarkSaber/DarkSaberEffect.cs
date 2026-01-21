using System;
using UnityEngine;

public class DarkSaberEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _blind;
    [SerializeField] private float _increaseDamageAmount = 0.5f;

    private WeaponBehaviour _darkSaber;

    private void Awake()
    {
        _darkSaber = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_darkSaber.TargetCharacter)
        {
            _darkSaber.TargetCharacter.OnNewBuffApplied -= TargetCharacter_OnNewBuffApplied;
        }

        _darkSaber.ResetWeaponStatsToDefault();
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_darkSaber == weapon)
        {
            TryApplyBlind();
        }
    }


    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_darkSaber.TargetCharacter)
        {
            _darkSaber.TargetCharacter.OnNewBuffApplied += TargetCharacter_OnNewBuffApplied;
        }
    }

    private void TargetCharacter_OnNewBuffApplied(Buff buff)
    {
        if (buff.Type == Buff.BuffType.Cold
           || buff.Type == Buff.BuffType.Poison
           || buff.Type == Buff.BuffType.Blindness)
        {
            IncreaseDamage();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void TryApplyBlind()
    {
        int manaStacks = _darkSaber.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana);       
        
        if (manaStacks > 0)
        {
            _darkSaber.TargetCharacter.ApplyBuff(_blind);
            _darkSaber.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, 1);
        }
    }

    private void IncreaseDamage()
    {
        _darkSaber.AddDamageToWeapon(_increaseDamageAmount);
    }
}
