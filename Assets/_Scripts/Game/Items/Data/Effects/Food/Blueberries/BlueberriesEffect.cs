using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BlueberriesEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 3.5f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _manaBuff;
    [SerializeField] private Buff _accuracyBuff;
    [SerializeField] private int _switchBuffsAmount = 10;

    private Character _owner;
    private Coroutine _blueberriesRoutine;


    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += Combatmanager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= Combatmanager_OnCombatFinished;

        if (_blueberriesRoutine != null)
            StopCoroutine(_blueberriesRoutine);
    }

    private void Combatmanager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_blueberriesRoutine != null)
        {
            StopCoroutine(_blueberriesRoutine);
            _blueberriesRoutine = null;
        }
        CooldownMultiplier = 1f;
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (item == null || targetCharacter == null)
            return;

        _owner = targetCharacter;

        ApplyBlueberriesBuff();


        if (_blueberriesRoutine != null)
            StopCoroutine(_blueberriesRoutine);

        _blueberriesRoutine = StartCoroutine(BlueberriesRoutine());
    }

    public void RemoveEffect()
    {
        if (_blueberriesRoutine != null)
        {
            StopCoroutine(_blueberriesRoutine);
            _blueberriesRoutine = null;
        }
    }

    private IEnumerator BlueberriesRoutine()
    {
        while (true)
        {
            yield return new WaitForCooldown(this);
            ApplyBlueberriesBuff();
            OnActivate();
        }
    }


    private void ApplyBlueberriesBuff()
    {
        if (_owner.GetBuffStacks(Buff.BuffType.Mana) > _switchBuffsAmount)
        {
            _owner.ApplyBuff(_accuracyBuff);
        }
        else
        {
            _owner.ApplyBuff(_manaBuff);
        }
    }

    public void TriggerFoodEffect()
    {
        ApplyBlueberriesBuff();
        OnActivate();
    }


    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
      
}
