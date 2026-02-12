using System;
using UnityEngine;

public class PestilanceFlaskEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private Buff _selfInflictedPoison;
    [SerializeField] private Buff _inflictedPoison;

    private bool _canConsume = true;

    private ItemBehaviour _pestilanceFlask;

    private void Awake()
    {
        _pestilanceFlask = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _pestilanceFlask.TargetCharacter.OnHealingRecived -= TargetCharacter_OnHealingRecived;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _pestilanceFlask.TargetCharacter.OnHealingRecived -= TargetCharacter_OnHealingRecived;
        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_pestilanceFlask == null || _pestilanceFlask.TargetCharacter == null) return;

        _pestilanceFlask.TargetCharacter.OnHealingRecived += TargetCharacter_OnHealingRecived;

    }

    private void TargetCharacter_OnHealingRecived(float obj)
    {
        if (_canConsume)
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
        _pestilanceFlask.TargetCharacter.ApplyBuff(_inflictedPoison);
        _pestilanceFlask.OwnerCharacter.ApplyBuff(_selfInflictedPoison);

        OnActivate();
        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _pestilanceFlask.TargetCharacter.ApplyBuff(_inflictedPoison);
        _pestilanceFlask.OwnerCharacter.ApplyBuff(_selfInflictedPoison);

        OnActivate();
    }
}
