using UnityEngine;

public class BowAndArrowStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _damageIncreaseAmount = 1f;

    [SerializeField] private int _currentDamageIncrease = 0;
    [SerializeField] private int _damageIncreaseAmountMax = 7;

    private WeaponBehaviour _trackedWeapon;
    private WeaponBehaviour _bowAndArow;

    private void Awake()
    {
        _bowAndArow = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;

    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour damageWeapon, string arg2)
    {
        if (damageWeapon == null)
            return;
        if (_trackedWeapon == null)
            return;


        if (damageWeapon == _trackedWeapon)
        {
            IncreaseDamage();
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _trackedWeapon = targetItem as WeaponBehaviour;

    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _trackedWeapon = null;
    }

    private void IncreaseDamage()
    {
        if (_currentDamageIncrease < _damageIncreaseAmountMax)
        {
            _currentDamageIncrease++;
            _bowAndArow.AddDamageToWeapon(_currentDamageIncrease);
        }
        else
        {
            _bowAndArow.AddDamageToWeapon(_currentDamageIncrease);
        }
    }
}
