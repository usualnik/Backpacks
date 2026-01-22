using System;
using UnityEngine;

public class ClawsOfAttackEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private Buff _empowerBuff;
    [SerializeField] private int _hitsCounter;
    [SerializeField] private float _increaseSpeedMiltiplier = 0.05f;

    private WeaponBehaviour _weaponBehaviour;

    private const int HITS_NEEDED_TO_APPLY_BUFF = 4;

    public event Action OnEffectAcivate;

    public int ItemActivations { get; set; }

    private void Awake()
    {
        _weaponBehaviour = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

        if (_weaponBehaviour.OwnerCharacter)
            _weaponBehaviour.OwnerCharacter.OnNewBuffApplied += OwnerCharacter_OnNewBuffApplied;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;     
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_weaponBehaviour.OwnerCharacter)
            _weaponBehaviour.OwnerCharacter.OnNewBuffApplied -= OwnerCharacter_OnNewBuffApplied;
    }



    private void OwnerCharacter_OnNewBuffApplied(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Thorns)
        {
            RecalculateSpeed();
        }
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ResetHitsCounterAfterCombat();
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character target, float arg3)
    {
        if (_weaponBehaviour == weapon)
        {
            _hitsCounter++;

            if (_hitsCounter == HITS_NEEDED_TO_APPLY_BUFF)
            {
                ApplyEmpowerBuff();
                _hitsCounter = 0;
            }
        }
    }

    private void ApplyEmpowerBuff()
    {
        if (_weaponBehaviour.OwnerCharacter)
        {
            _weaponBehaviour.OwnerCharacter.ApplyBuff(_empowerBuff);
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

    private void ResetHitsCounterAfterCombat()
    {
        _hitsCounter = 0;
    }

    private void RecalculateSpeed()
    {
        if (_weaponBehaviour.OwnerCharacter == null) return;
        int activeThornsStacks = _weaponBehaviour.OwnerCharacter.GetBuffStacks(Buff.BuffType.Thorns);

        if (activeThornsStacks > 0)
        _weaponBehaviour.IncreaseSpeedMultiplier(_increaseSpeedMiltiplier * activeThornsStacks);


    }

}
