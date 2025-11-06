using UnityEngine;

public class WeaponBehaviour : ItemBehaviour
{
    public float WeaponDamageMin => _weaponDamageMin;
    public float WeaponDamageMax => _weaponDamageMax;

    [SerializeField] private WeaponDataSO _weaponDataSO;
    [SerializeField] private float _weaponDamageMin;
    [SerializeField] private float _weaponDamageMax;


    private void Awake()
    {
        _weaponDamageMin = _weaponDataSO.DamageMin;
        _weaponDamageMax = _weaponDataSO.DamageMax;
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
            //HACK: Применяется только первый эффект в списке - это неверно
            _weaponDataSO.Effects[0].ApplyEffect(_target);
        }
    }

    public void AddDamageToWeapon(float value)
    {
        _weaponDamageMin += value;
        _weaponDamageMax += value;
    }

}
