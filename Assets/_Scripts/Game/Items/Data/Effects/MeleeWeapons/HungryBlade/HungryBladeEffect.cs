using System;
using UnityEngine;

public class HungryBladeEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _vampirismBuff;
    [SerializeField] private float _damageBuffPerVampStack = 1f;

    private WeaponBehaviour _hungryBlade;

    private void Awake()
    {
        _hungryBlade = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _hungryBlade.OwnerCharacter.OnNewBuffApplied -= OwnerCharacter_OnNewBuffApplied;
        _hungryBlade.ResetWeaponStatsToDefault();
    }


    private void CombatManager_OnCombatStarted()
    {
        StartOfBattleVampBuf();
    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_hungryBlade != weapon) return;

        if(_hungryBlade.OwnerCharacter.GetBuffStacks(Buff.BuffType.Regeneration) > 0)
        {
            OnHitEffect();
            OnActivate();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _hungryBlade.OwnerCharacter.OnNewBuffApplied += OwnerCharacter_OnNewBuffApplied;
    }

    private void OwnerCharacter_OnNewBuffApplied(Buff buff)
    {
        if (buff.Type == Buff.BuffType.Vampirism)
        {
            _hungryBlade.AddDamageToWeapon(_damageBuffPerVampStack);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void StartOfBattleVampBuf()
    {
        if (!_hungryBlade.OwnerCharacter) return;

        _hungryBlade.OwnerCharacter.ApplyBuff(_vampirismBuff);
        OnActivate();
    }
    private void OnHitEffect()
    {
        _hungryBlade.OwnerCharacter.RemoveBuff(Buff.BuffType.Regeneration, 1);
        _hungryBlade.OwnerCharacter.ApplyBuff(_vampirismBuff);
    }
    
}
