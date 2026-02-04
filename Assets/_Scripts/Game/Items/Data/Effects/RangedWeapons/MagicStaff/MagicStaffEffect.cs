using System;
using UnityEngine;

public class MagicStaffEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _additionalDamageAmount = 6f;
    [SerializeField] private float _damageIncreaseAmount = 2f;
    [SerializeField] private int _manaStacksToProcEffectNeeded = 3;


    private WeaponBehaviour _magicStaff;

    private void Awake()
    {
        _magicStaff = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;

    }
    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_magicStaff.OwnerCharacter == null) return;


        if (_magicStaff == weapon 
            && _magicStaff.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) >= _manaStacksToProcEffectNeeded)
        {
            ProcEffect();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    private void ProcEffect()
    {
        _magicStaff.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, _manaStacksToProcEffectNeeded);
        _magicStaff.TargetCharacter.TakeDamage(_additionalDamageAmount, ItemDataSO.ExtraType.Ranged);
        _magicStaff.AddDamageToWeapon(_damageIncreaseAmount);

        OnActivate();
    }

}
