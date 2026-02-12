using System;
using UnityEngine;

public class HealthPotionEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private float _healAmount = 12f;
    [SerializeField] private int _removePoisonAmount = 4;

    private bool _canConsume = true;
    private float _healthToTriggerConsume;

    private ItemBehaviour _healthPotion;

    private void Awake()
    {
        _healthPotion = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _healthPotion.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _healthPotion.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_healthPotion == null || _healthPotion.OwnerCharacter == null) return;

        _healthToTriggerConsume = _healthPotion.OwnerCharacter.Stats.Health / 2;

        _healthPotion.OwnerCharacter.OnCharacterStatsChanged += Owner_OnCharacterStatsChanged;
    }

    private void Owner_OnCharacterStatsChanged(Character.CharacterStats stats)
    {
        if (stats.Health <= _healthToTriggerConsume && _canConsume)
        {
            Consume();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void Consume()
    {
        _healthPotion.OwnerCharacter.AddHealth(_healAmount);
        _healthPotion.OwnerCharacter.RemoveBuff(Buff.BuffType.Poison, _removePoisonAmount);

        OnActivate();

        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _healthPotion.OwnerCharacter.AddHealth(_healAmount);
        _healthPotion.OwnerCharacter.RemoveBuff(Buff.BuffType.Poison, _removePoisonAmount);

        OnActivate();
    }
}
