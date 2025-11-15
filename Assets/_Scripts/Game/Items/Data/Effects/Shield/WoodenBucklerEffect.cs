using UnityEngine;

public class WoodenBucklerEffect : MonoBehaviour, IItemEffect, IDamagePreventionEffect
{
    [SerializeField]
    private float _chanceToPreventDamage = 30f;
    [SerializeField]
    private float _preventedDamageAmount = 7f;
    [SerializeField]
    private float _removedStaminaAmount = 0.3f;

    private Character _targetCharacter;


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