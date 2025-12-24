using UnityEngine;

public class PumpkinEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private float _chanceToStun = 50f;
    [SerializeField]
    private float _stunDuration = 0.5f;

    [SerializeField]
    private Buff _pumpkinBuff;

    private WeaponBehaviour _pumpkinWeapon;

    private void Awake()
    {
        _pumpkinWeapon = GetComponent<WeaponBehaviour>();
        if (_pumpkinWeapon == null)
            _pumpkinWeapon = GetComponentInParent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnFatigueDamageStarted += CombatManager_OnFatigueDamageStarted;
    }    

    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnFatigueDamageStarted -= CombatManager_OnFatigueDamageStarted;

    }
    private void CombatManager_OnFatigueDamageStarted()
    {
        _pumpkinWeapon.OwnerCharacter?.ApplyBuff(_pumpkinBuff);
    }
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour attackedWeapon, string attackedTargetName)
    {
        if (_pumpkinWeapon != null && attackedWeapon == _pumpkinWeapon)
        {
            TryApplyStun(attackedWeapon, attackedTargetName);
        }
    }

    private void TryApplyStun(WeaponBehaviour weapon, string targetName)
    {
        if (Random.Range(0f, 100f) <= _chanceToStun)
        {
            Character targetCharacter = GetTargetCharacter(targetName);
            if (targetCharacter != null)
            {
                CombatManager.Instance.StunCharacter(targetCharacter, _stunDuration);
            }
        }
    }

    private Character GetTargetCharacter(string targetName)
    {
        if (targetName == CombatManager.Instance.GetPlayerCharacter().name)
            return CombatManager.Instance.GetPlayerCharacter();
        else if (targetName == CombatManager.Instance.GetEnemyCharacter().name)
            return CombatManager.Instance.GetEnemyCharacter();
        
        return null;
    }

    public void RemoveEffect()
    {
        // Логика удаления эффекта
    }
}