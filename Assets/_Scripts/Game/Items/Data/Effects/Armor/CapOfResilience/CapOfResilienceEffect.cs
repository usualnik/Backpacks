using System;
using UnityEngine;

public class CapOfResilienceEffect : MonoBehaviour, IItemEffect, IDamagePreventionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _chanceToPreventDamage = 25f;
    [SerializeField] private float _preventedDamageAmount = 9f;
    [SerializeField] private float _critResistChance = 15f;
    [SerializeField] private float _stunResistChance = 15f;


    private ItemBehaviour _cap;

    private void Awake()
    {
        _cap = GetComponent<ItemBehaviour>();
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
        _chanceToPreventDamage = 25f;
    }
    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        var damageHandler = _cap.OwnerCharacter.GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.RegisterMeleeDamagePreventionEffect(this);
        }

        Invoke(nameof(StopPreventing), 3f);

        _cap.OwnerCharacter.AddCriticalHitResistChance(_critResistChance);
        _cap.OwnerCharacter.AddStunResistChance(_stunResistChance);
    }

    public bool ShouldPreventDamage()
    {
        bool isPreventing = UnityEngine.Random.Range(0f, 100f) <= _chanceToPreventDamage;

        if (isPreventing)
        {
            OnActivate();
        }

        return isPreventing;
    }

    public float GetPreventedDamageAmount()
    {
        return _preventedDamageAmount;
    }
    public void RemoveEffect()
    {
        var damageHandler = _cap.OwnerCharacter.GetComponent<CharacterDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.UnRegisterMeleeDamagePreventionEffect(this);
        }
    }

    private void StopPreventing()
    {
        _chanceToPreventDamage = 0f;
    }

}
