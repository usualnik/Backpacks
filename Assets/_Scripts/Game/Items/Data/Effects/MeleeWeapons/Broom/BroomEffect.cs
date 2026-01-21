using System;
using UnityEngine;

public class BroomEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }


    [SerializeField] private Buff _blindBuff;
    [SerializeField] private float _chanceToInflictBlind = 33f;
    [SerializeField] private float _nextAttackDamageBuff = 2f;


    private WeaponBehaviour _weaponBehaviour;

    private bool _isDamageBuffed = false;



    private void Awake()
    {
        _weaponBehaviour = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnMiss += CombatManager_OnMiss;
    }

    private void CombatManager_OnMiss(WeaponBehaviour weapon, Character missedCharacter)
    {
        if (_weaponBehaviour.TargetCharacter == missedCharacter && !_isDamageBuffed)
        {
            DamageBuff();
            OnActivate();
        }
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnMiss -= CombatManager_OnMiss;
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;

    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character target, float arg3)
    {
        if (_weaponBehaviour == weapon)
        {
            TryApplyBuff(target);

            if (_isDamageBuffed)
            {
                RemoveDamageBuff();
            }
        }
    }

    private void TryApplyBuff(Character target)
    {
        bool isProc = UnityEngine.Random.Range(1, 100) <= _chanceToInflictBlind ? true : false;

        if (isProc)
        {
            target.ApplyBuff(_blindBuff);
            OnActivate();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void RemoveDamageBuff()
    {
        _weaponBehaviour.AddDamageToWeapon(-_nextAttackDamageBuff);
        _isDamageBuffed = false;
    }
    private void DamageBuff()
    {
        _weaponBehaviour.AddDamageToWeapon(_nextAttackDamageBuff);
        _isDamageBuffed = true;
    }
}
