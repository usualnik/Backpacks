using System;
using UnityEngine;

public class DragonskinBootsEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private Buff _luckBuff;
    [SerializeField] private Buff _empowerBuff;

    private bool _canTrigger = true;
    private float _healthToTriggerEffect;

    private ItemBehaviour _boots;

    private void Awake()
    {
        _boots = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _boots.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _boots.OwnerCharacter.OnCharacterStatsChanged -= Owner_OnCharacterStatsChanged;
        _canTrigger = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_boots == null || _boots.OwnerCharacter == null) return;

        _healthToTriggerEffect = _boots.OwnerCharacter.Stats.Health / 1.5f;

        _boots.OwnerCharacter.OnCharacterStatsChanged += Owner_OnCharacterStatsChanged;
    }

    private void Owner_OnCharacterStatsChanged(Character.CharacterStats stats)
    {
        if (stats.Health <= _healthToTriggerEffect && _canTrigger)
        {
            TriggerEffect();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerEffect()
    {
        _boots.OwnerCharacter.ApplyBuff(_luckBuff);
        _boots.OwnerCharacter.ApplyBuff(_empowerBuff);

        OnActivate();

        OnPotionConsumed?.Invoke();
        _canTrigger = false;
    }

   
}
