using System;
using System.Collections;
using UnityEngine;

public class CritwoodStaffEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _damageBuff = 7f;
    [SerializeField] private float _buffUptime = 1.2f;
    [SerializeField] private int _manaToProcNeeded = 3;
    
    private const float HUNDRED_PERCENT_CRIT = 100f;

    private WeaponBehaviour _critWoodStaff;

    private void Awake()
    {
        _critWoodStaff = GetComponent<WeaponBehaviour>();
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
        if (_critWoodStaff != weapon) return;

        if (!_critWoodStaff.OwnerCharacter) return;
        
        if (_critWoodStaff.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) > _manaToProcNeeded)
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
        _critWoodStaff.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, _manaToProcNeeded);
        StartCoroutine(BuffDamageAndCrit());
    }

    private IEnumerator BuffDamageAndCrit()
    {
        _critWoodStaff.AddDamageToWeapon(_damageBuff);
        _critWoodStaff.AddCritHitChanceToWeapon(HUNDRED_PERCENT_CRIT);

        yield return new WaitForSeconds(_buffUptime);

        if (CombatManager.Instance.IsInCombat)
        {
            _critWoodStaff.AddDamageToWeapon(-_damageBuff);
            _critWoodStaff.AddCritHitChanceToWeapon(-HUNDRED_PERCENT_CRIT);
        }
        
    }
}
