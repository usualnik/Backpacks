using UnityEngine;

public class SpikedShieldEfect : MonoBehaviour, IItemEffect, IDamagePreventionEffect
{
    private float _chanceToPreventDamage = 30f;
    private float _preventedDamageAmount = 9f;
    private float _removedStaminaAmount = 0.3f;
    
    private int _gainedThornsAmount = 0;

    private const int MAX_GAINED_THORNS_PER_PROC = 5;


    private Character _targetCharacter;
    private Character _sourceCharacter;

    private Buff _spikedShieldBuff;

    private void Awake()
    {
        _spikedShieldBuff = new Buff
        {
            Name = "SpikedShieldBuff",
            Type = Buff.BuffType.Thorns,
            IsPositive = true,
            Value = 1
        };
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

            if (_gainedThornsAmount <= MAX_GAINED_THORNS_PER_PROC)
            {
                _sourceCharacter.ApplyBuff(_spikedShieldBuff);
            }
        }

        return isPreventing;

    }

    public float GetPreventedDamageAmount()
    {
        return _preventedDamageAmount;
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        //HACK: Ќужно лучше расписать систему таргетов, иначе такие записи станут просто не читабельны
        _targetCharacter = sourceCharacter;
        _sourceCharacter = targetCharacter;


        var damageHandler = targetCharacter.GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.RegisterMeleeDamagePreventionEffect(this);
        }
    }

    public void RemoveEffect()
    {
        var damageHandler = GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.UnRegisterMeleeDamagePreventionEffect(this);
        }
    }
}
