using System;
using UnityEngine;

public class DemonicFlaskEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private float _damagePerDebuff = 0.45f;

    private bool _canConsume = true;
    private float _healthToTriggerConsume;

    private ItemBehaviour _demonicFlask;

    private void Awake()
    {
        _demonicFlask = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _demonicFlask.OwnerCharacter.OnCharacterStatsChanged -= Target_OnCharacterStatsChanged;
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _demonicFlask.OwnerCharacter.OnCharacterStatsChanged -= Target_OnCharacterStatsChanged;
        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_demonicFlask == null || _demonicFlask.TargetCharacter == null) return;

        _healthToTriggerConsume = _demonicFlask.TargetCharacter.Stats.Health / 2;

        _demonicFlask.TargetCharacter.OnCharacterStatsChanged += Target_OnCharacterStatsChanged;
    }

    private void Target_OnCharacterStatsChanged(Character.CharacterStats stats)
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
        var allOpponentDebuffs = _demonicFlask.TargetCharacter.AllDebuffs.Count;

        float damage = allOpponentDebuffs * _damagePerDebuff;

        _demonicFlask.TargetCharacter.TakeDamage(damage,ItemDataSO.ExtraType.Effect);

        OnActivate();

        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        var allOpponentDebuffs = _demonicFlask.TargetCharacter.AllDebuffs.Count;

        float damage = allOpponentDebuffs * _damagePerDebuff;

        _demonicFlask.TargetCharacter.TakeDamage(damage, ItemDataSO.ExtraType.Effect);

        OnActivate();

    }
}
