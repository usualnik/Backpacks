using UnityEngine;

public class WeaponBehaviour : ItemBehaviour
{
    public float WeaponDamageMin => _weaponDamageMin;
    public float WeaponDamageMax => _weaponDamageMax;
    public float CritHitChance => _critHitChance;

    [SerializeField] private WeaponDataSO _weaponDataSO;

    private float _weaponDamageMin;
    private float _weaponDamageMax;

    [SerializeField]
    private float _critHitChance = 0;  


    private void Awake()
    {
        _weaponDamageMin = _weaponDataSO.DamageMin;
        _weaponDamageMax = _weaponDataSO.DamageMax;
        _critHitChance = _weaponDataSO.BaseCritChance;
    }
    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;

    }

    private void CombatManager_OnCombatStarted()
    {
        if (CurrentState.HasFlag(ItemState.Inventory))
        {
            _weaponDataSO.PerformWeaponAction(_target, this);           
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

}
