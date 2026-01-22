using System;
using UnityEngine;

public class SpectralDaggerEffect : MonoBehaviour,IItemEffect
{
    public int ItemActivations { get; set; } = 0;
    public event Action OnEffectAcivate;

    [SerializeField] private float _additionalDamage = 7f;

    private WeaponBehaviour _daggerBehaviour;


    private void Awake()
    {
        _daggerBehaviour = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCharacterStuned += CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnHit -= CombatManager_OnHit;

    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_daggerBehaviour != weapon) return;

        if (_daggerBehaviour.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) > 0)
        {
            DealAdditionalDamage();
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
            OnActivate();
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

    private void DealAdditionalDamage()
    {
        if(_daggerBehaviour.TargetCharacter == null) return;

        _daggerBehaviour.TargetCharacter.TakeDamage(_additionalDamage, ItemDataSO.ExtraType.Effect);
    }
}
