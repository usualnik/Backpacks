using System;
using UnityEngine;

public class BloodyDaggerEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; } = 0;
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _vampirismBuff;

    private WeaponBehaviour _daggerBehaviour;

    private int _vamprirismGainedAmount = 0;
    private const int VAMPRIRISM_GAINED_MAX = 5;
    private Character _owner;

    private void Awake()
    {
        _daggerBehaviour = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnCharacterStuned += CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt += CombatMnager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt -= CombatMnager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ResetVampirismGainedAfterCombat();
    }

    private void CombatMnager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_daggerBehaviour == weapon)
        {
            OnDaggerHitEffect();
            OnActivate();
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
        _owner = sourceCharacter;
    }

    public void RemoveEffect()
    {
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();

    }

    private void OnDaggerHitEffect()
    {
        if (_vamprirismGainedAmount >= VAMPRIRISM_GAINED_MAX) return;
        if (_owner == null) return;

        _vamprirismGainedAmount++;
        _owner.ApplyBuff(_vampirismBuff);
    }

    private void ResetVampirismGainedAfterCombat()
    {
        _vamprirismGainedAmount = 0;
    }
}
