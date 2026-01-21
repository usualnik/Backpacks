using System;
using UnityEngine;

public class CrossBladesEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private float _damageIncrease = 0;
    [SerializeField] private float _speedIncrease = 0.04f;

    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    private WeaponBehaviour _weaponBehaviour;

    private void Awake()
    {
        _weaponBehaviour = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _weaponBehaviour.ResetWeaponStatsToDefault();
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_weaponBehaviour == weapon)
        {
            ApplyCrossBladesEffect();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    private void ApplyCrossBladesEffect()
    {
        _damageIncrease++;

        _weaponBehaviour.AddDamageToWeapon(_damageIncrease);
        _weaponBehaviour.IncreaseSpeedMultiplier(_speedIncrease);
        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }


}
