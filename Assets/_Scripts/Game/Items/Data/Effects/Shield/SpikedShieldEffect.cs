using UnityEngine;

public class SpikedShieldEffect : MonoBehaviour, IItemEffect, IDamagePreventionEffect
{

    public event System.Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    [SerializeField]
    private float _chanceToPreventDamage = 30f;
    [SerializeField]
    private float _preventedDamageAmount = 9f;
    [SerializeField]
    private float _removedStaminaAmount = 0.3f;
    [SerializeField]
    private int _maxgainedThornsPerProc = 5;

    private int _gainedThornsAmount = 0;

    private Character _targetCharacter;
    private Character _sourceCharacter;

    [SerializeField]
    private Buff _spikedShieldBuff;

    private ItemBehaviour _spikedShield;

    private void Awake()
    {
        _spikedShield = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        RemoveEffect();
    }

    public bool ShouldPreventDamage()
    {
        bool isPreventing = Random.Range(0f, 100f) <= _chanceToPreventDamage;

        if (isPreventing)
        {
            _targetCharacter.UseStamina(_removedStaminaAmount);


            _gainedThornsAmount++;

            if (_gainedThornsAmount <= _maxgainedThornsPerProc)
            {
                _sourceCharacter.ApplyBuff(_spikedShieldBuff);
                OnActivate();
            }
        }

        return isPreventing;

    }

    public float GetPreventedDamageAmount()
    {
        return _preventedDamageAmount;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
       
        var damageHandler = _spikedShield.OwnerCharacter.GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.RegisterMeleeDamagePreventionEffect(this);
        }
    }

    public void RemoveEffect()
    {
        var damageHandler = _spikedShield.OwnerCharacter.GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.UnRegisterMeleeDamagePreventionEffect(this);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
