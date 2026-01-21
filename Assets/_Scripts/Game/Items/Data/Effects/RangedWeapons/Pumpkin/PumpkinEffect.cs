using UnityEngine;

public class PumpkinEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event System.Action OnEffectAcivate;

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
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnFatigueDamageStarted += CombatManager_OnFatigueDamageStarted;
    }    

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
        CombatManager.Instance.OnFatigueDamageStarted -= CombatManager_OnFatigueDamageStarted;

    }
    private void CombatManager_OnFatigueDamageStarted()
    {
        _pumpkinWeapon.OwnerCharacter?.ApplyBuff(_pumpkinBuff);
        OnActivate();
    }
    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour attackedWeapon, Character character, float damage)
    {
        if (_pumpkinWeapon != null && attackedWeapon == _pumpkinWeapon)
        {
            TryApplyStun(attackedWeapon, character);
        }
    }

    private void TryApplyStun(WeaponBehaviour weapon, Character targetCharacter)
    {
        if (Random.Range(0f, 100f) <= _chanceToStun)
        {
            if (targetCharacter != null)
            {
                CombatManager.Instance.StunCharacter(targetCharacter, _stunDuration);
                OnActivate();

            }
        }
    }

  
    public void RemoveEffect()
    {
        // Логика удаления эффекта
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}