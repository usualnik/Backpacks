using System;
using UnityEngine;

public class StrongHeroicPotionEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private int _staminaBuff = 4;
    [SerializeField] private Buff _empowerBuff;

    private bool _canConsume = true;

    private ItemBehaviour _heroicPotion;

    private void Awake()
    {
        _heroicPotion = GetComponent<ItemBehaviour>();
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
        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_heroicPotion == null || _heroicPotion.OwnerCharacter == null) return;

        _heroicPotion.OwnerCharacter.OnStaminaEmpty += OwnerCharacter_OnStaminaEmpty;
    }

    private void OwnerCharacter_OnStaminaEmpty()
    {
        if (_canConsume)
            TriggerEffect();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerEffect()
    {
        _heroicPotion.OwnerCharacter.AddStamina(_staminaBuff);
        _heroicPotion.OwnerCharacter.ApplyBuff(_empowerBuff);

        OnActivate();
        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _heroicPotion.OwnerCharacter.AddStamina(_staminaBuff);
        _heroicPotion.OwnerCharacter.ApplyBuff(_empowerBuff);

        OnActivate();
    }
}
