using System;
using UnityEngine;

public class BloodThorneEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _vamprirismBuff;
    [SerializeField] private Buff _thornsBuff;

    private Character _owner;
    private WeaponBehaviour _bloodThorne;

    private float _damageIncreaseAmount = 0;

    private void Awake()
    {
        _bloodThorne = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
    }
  
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
    }


    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_bloodThorne == weapon)
        {
            ApplyBloodThorneEffect();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _owner = sourceCharacter;
    }

    private void ApplyBloodThorneEffect()
    {
        if (_owner == null) return;
        if (_owner.GetBuffStacks(Buff.BuffType.Regeneration) == 0) return;

        _owner.RemoveBuff(Buff.BuffType.Regeneration, 1);

        _owner.ApplyBuff(_vamprirismBuff);
        _owner.ApplyBuff(_thornsBuff);

        OnActivate();

        IncreaseWeaponDamage();
    }

    private void IncreaseWeaponDamage()
    {
        int increaseAmount = _owner.GetBuffStacks(Buff.BuffType.Vampirism) + _owner.GetBuffStacks(Buff.BuffType.Thorns);
        _bloodThorne.AddDamageToWeapon(increaseAmount);

        _damageIncreaseAmount += increaseAmount;
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

}
