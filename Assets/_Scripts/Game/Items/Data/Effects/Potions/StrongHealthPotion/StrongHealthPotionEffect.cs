using System;
using UnityEngine;

public class StrongHealthPotionEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private float _healAmount = 24f;
    [SerializeField] private Buff _regenBuff;
    [SerializeField] private int _removePoisonAmount = 4;

    private bool _canConsume = true;
    private float _healthToTriggerConsume;

    private ItemBehaviour _strongHealthPotion;

    private void Awake()
    {
        _strongHealthPotion = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _strongHealthPotion.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _strongHealthPotion.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_strongHealthPotion == null || _strongHealthPotion.OwnerCharacter == null) return;

        _healthToTriggerConsume = _strongHealthPotion.OwnerCharacter.Stats.Health / 2;

        _strongHealthPotion.OwnerCharacter.OnCharacterStatsChanged += Owner_OnCharacterStatsChanged;
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
        _strongHealthPotion.OwnerCharacter.AddHealth(_healAmount);
        _strongHealthPotion.OwnerCharacter.ApplyBuff(_regenBuff);
        _strongHealthPotion.OwnerCharacter.RemoveBuff(Buff.BuffType.Poison, _removePoisonAmount);
        OnActivate();

        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _strongHealthPotion.OwnerCharacter.AddHealth(_healAmount);
        _strongHealthPotion.OwnerCharacter.ApplyBuff(_regenBuff);
        _strongHealthPotion.OwnerCharacter.RemoveBuff(Buff.BuffType.Poison, _removePoisonAmount);
        OnActivate();
    }
}
