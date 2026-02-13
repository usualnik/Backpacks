using System;
using UnityEngine;

public class WeaponBehaviour : ItemBehaviour, ICooldownable
{
    public event Action OnWeaponStatsChanged;
    public WeaponDataSO WeaponDataSO => _weaponDataSO;

    // ICooldownable__________________________________________________

    public float BaseCooldown { get; private set; }
    public float CooldownMultiplier { get; set; } = 1;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    //_________________________________________________________________

    public float WeaponDamageMin => _weaponDamageMin;
    public float WeaponDamageMax => _weaponDamageMax;
    public float CritHitChance => _critHitChance;
    public float Accuracy => _accuracy;
    public float StaminaCost => _staminaCost;


    [SerializeField] private WeaponDataSO _weaponDataSO;
    [SerializeField] private bool _canAttack = true;

    private float _weaponDamageMin;
    private float _weaponDamageMax;

    private float _critHitChance = 0; 

    private float _accuracy = 0;
    public float _staminaCost = 0;


    private WeaponBehaviour _weaponBehaviour;


    private void Awake()
    {
        _ownerTargetHandler = GetComponent<OwnerTargetHandler>();
        ResetWeaponStatsToDefault();
        _effect = GetComponent<IItemEffect>();
        _weaponBehaviour = this;
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        _weaponBehaviour.OnWeaponStatsChanged += Weapon_OnWeaponStatsChangedDuringCombat;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

        base.ConfigureItemOwnerTarget();
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        _weaponBehaviour.OnWeaponStatsChanged -= Weapon_OnWeaponStatsChangedDuringCombat;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ResetWeaponStatsToDefault();
    }

    private void Weapon_OnWeaponStatsChangedDuringCombat()
    {
        if (CombatManager.Instance.IsInCombat)
        {
            RestartAutoAttackWithNewStats();
        }
    }

    private void CombatManager_OnCombatStarted()
    {
        StartAutoAttack();

        if (_effect != null)
        {
            _effect?.StartOfCombatInit(this, _ownerCharacter, _targetCharacter);
        }
    }

    public void AddDamageToWeapon(float value)
    {
        _weaponDamageMin += value;
        _weaponDamageMax += value;

        OnWeaponStatsChanged?.Invoke();
    }

    public void AddCritHitChanceToWeapon(float value)
    {
        _critHitChance += value;
        OnWeaponStatsChanged?.Invoke();
    }

    public void AddAccuracyToWeapon(float value)
    {
        _accuracy += value;
        OnWeaponStatsChanged?.Invoke();
    }

    public void RemoveStaminaUsage(float value)
    {
        _staminaCost -= value;
        OnWeaponStatsChanged?.Invoke();
    }

    public void IsCanAutoAttack(bool canAutoAttack)
    {
        _canAttack = canAutoAttack;

        if (_canAttack)
        {
            StartAutoAttack();
        }
    }


    private void ResetWeaponStatsToDefault()
    {
        BaseCooldown = _weaponDataSO.Cooldown;
        CooldownMultiplier = 1f;

        _weaponDamageMin = _weaponDataSO.DamageMin;
        _weaponDamageMax = _weaponDataSO.DamageMax;
        _critHitChance = _weaponDataSO.BaseCritChance;       
        _accuracy = _weaponDataSO.Accuracy;
        _staminaCost = _weaponDataSO.StaminaCost;
    }

    private void StartAutoAttack()
    {
        if (CurrentState.HasFlag(ItemState.Inventory) && _canAttack)
        {
            CombatManager.Instance.StartAutoAttack(GetTarget(), this,
            WeaponDamageMin, WeaponDamageMax,
            StaminaCost, CurrentCooldown, Accuracy);
        }
    }

    private void RestartAutoAttackWithNewStats()
    {
        //Тормозим текущую автоатаку
        CombatManager.Instance.StopSpecificAutoAttack(this);

        //Перезапускаем с новыми параметрами
        if (CurrentState.HasFlag(ItemState.Inventory) && _canAttack)
        {
            CombatManager.Instance.StartAutoAttack(GetTarget(), this,
            WeaponDamageMin, WeaponDamageMax,
            StaminaCost, CurrentCooldown, Accuracy);
        }
    }
}
