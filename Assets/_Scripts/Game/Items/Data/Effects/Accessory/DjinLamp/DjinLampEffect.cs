using System;
using System.Collections;
using UnityEngine;

public class DjinLampEffect : MonoBehaviour, IItemEffect, ICooldownable
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 1.6f;
    public float CooldownMultiplier { get ; set ; } = 1f;

    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _manaBuff;
    [SerializeField] private Buff _thornsBuff;
    [SerializeField] private Buff _luckBuff;


    private Coroutine _djinLampRoutine;
    private ItemBehaviour _djinLamp;

    private void Awake()
    {
        _djinLamp = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_djinLampRoutine != null)
        {
            StopCoroutine(DjinLampRoutine());
            _djinLampRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_djinLampRoutine != null)
        {
            StopCoroutine(DjinLampRoutine());
            _djinLampRoutine = null;
        }

        CooldownMultiplier = 1f;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_djinLamp.OwnerCharacter == null) { return; }

        if (_djinLampRoutine == null)
        {
            _djinLampRoutine = StartCoroutine(DjinLampRoutine());
        }
    }

    private IEnumerator DjinLampRoutine()
    {
        while (true)
        {

            GainBuff();
            OnActivate();

            yield return new WaitForCooldown(this);
        }
    }

    private void GainBuff()
    {
        int manaStacks = _djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana);
        int thornsStacks = _djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Thorns);
        int luckStacks = _djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Luck);

        if (manaStacks <= thornsStacks && manaStacks <= luckStacks)
        {
            _djinLamp.OwnerCharacter.ApplyBuff(_manaBuff);
        }
        else if (thornsStacks <= manaStacks && thornsStacks <= luckStacks)
        {
            _djinLamp.OwnerCharacter.ApplyBuff(_thornsBuff);
        }
        else
        {
            _djinLamp.OwnerCharacter.ApplyBuff(_luckBuff);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
