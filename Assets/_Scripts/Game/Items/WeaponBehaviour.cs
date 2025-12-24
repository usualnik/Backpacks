using UnityEngine;

public class WeaponBehaviour : ItemBehaviour
{
    public WeaponDataSO WeaponDataSO => _weaponDataSO;
    public float WeaponDamageMin => _weaponDamageMin;
    public float WeaponDamageMax => _weaponDamageMax;
    public float CritHitChance => _critHitChance;

    [SerializeField] private WeaponDataSO _weaponDataSO;

    private float _weaponDamageMin;
    private float _weaponDamageMax;

    private float _critHitChance = 0;

    private float _coolDownSpeed = 0;
    private float _currentCooldownMultiplier = 1f;


    private void Awake()
    {
        _ownerTargetHandler = GetComponent<OwnerTargetHandler>();

        _weaponDamageMin = _weaponDataSO.DamageMin;
        _weaponDamageMax = _weaponDataSO.DamageMax;
        _critHitChance = _weaponDataSO.BaseCritChance;
        _coolDownSpeed = _weaponDataSO.Cooldown;
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;

        base.ConfigureItemOwnerTarget();
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;

    }

    private void CombatManager_OnCombatStarted()
    {
        if (CurrentState.HasFlag(ItemState.Inventory))
        {
            CombatManager.Instance.StartAutoAttack(GetTarget(), this,
            WeaponDamageMin, WeaponDamageMax,
            _weaponDataSO.StaminaCost, _coolDownSpeed / _currentCooldownMultiplier, _weaponDataSO.Accuracy);
        }
    }

    public void AddDamageToWeapon(float value)
    {
        _weaponDamageMin += value;
        _weaponDamageMax += value;
    }

    public void AddCritHitChanceToWeapon(float value)
    {
        _critHitChance += value;
    }

    public void IncreaseSpeedMultiplier(float value)
    {
        _currentCooldownMultiplier += value;       
    }

}
