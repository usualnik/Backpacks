using JetBrains.Annotations;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class DjinLampEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _cooldown = 1.6f;
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

            yield return new WaitForSeconds(_cooldown);
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
