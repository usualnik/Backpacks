using System;
using UnityEngine;

public class LightSaberEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _blindBuff;
    [SerializeField] private float _blindBuffCooldown = 6f;
    [SerializeField] private float _damagePerOpponentBlindBuff = 1f;
    [SerializeField] private int _regenerationNeededToProcBlindAmount = 3;

    private bool _isBuffApplied = false;

    private WeaponBehaviour _lightSaber;


    private void Awake()
    {
        _lightSaber = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ResetBuffCooldown();
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_lightSaber.OwnerCharacter == null || _lightSaber.TargetCharacter == null) return;

        if (_lightSaber == weapon)
        {
            ApplyLihghSaberEffect();
        }
    }


    private void ApplyLihghSaberEffect()
    {
        float additionalDamage = 0;

        if (_lightSaber.OwnerCharacter.GetBuffStacks(Buff.BuffType.Regeneration) 
            >= _regenerationNeededToProcBlindAmount && !_isBuffApplied)
        {
            _lightSaber.OwnerCharacter.RemoveBuff(Buff.BuffType.Regeneration,_regenerationNeededToProcBlindAmount);
            _lightSaber.TargetCharacter.ApplyBuff(_blindBuff);
            _isBuffApplied = true;
            Invoke(nameof(ResetBuffCooldown), _blindBuffCooldown);
        }

        additionalDamage = _lightSaber.TargetCharacter.GetBuffStacks(Buff.BuffType.Blindness) * _damagePerOpponentBlindBuff;

        _lightSaber.AddDamageToWeapon(additionalDamage);


    }
    private void ResetBuffCooldown()
    {
        _isBuffApplied = false;
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }


}
