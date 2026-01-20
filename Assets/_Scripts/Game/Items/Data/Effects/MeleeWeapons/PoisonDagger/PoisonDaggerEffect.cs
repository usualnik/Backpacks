using System;
using UnityEngine;

public class PoisonDaggerEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; } = 0;
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _poisonDebuff;

    private WeaponBehaviour _daggerBehaviour;

    private void Awake()
    {
        _daggerBehaviour = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnCharacterStuned += CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
    }


    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_daggerBehaviour == weapon)
        {
            ApplyPoisonDebuff();
        }
    }

    private void CombatManager_OnCharacterStuned(Character stunnedCharacter, float stunDuration)
    {
        if (!_daggerBehaviour.OwnerCharacter)
        {
            return;
        }

        if (stunnedCharacter != null && stunnedCharacter == _daggerBehaviour.OwnerCharacter)
        {
            CombatManager.Instance.AttackCharacterOnce(_daggerBehaviour.OwnerCharacter, _daggerBehaviour.TargetCharacter, _daggerBehaviour);
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void RemoveEffect()
    {
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();

    }

    private void ApplyPoisonDebuff()
    {
        _daggerBehaviour.TargetCharacter.ApplyBuff(_poisonDebuff);
        OnActivate();
    }
}
