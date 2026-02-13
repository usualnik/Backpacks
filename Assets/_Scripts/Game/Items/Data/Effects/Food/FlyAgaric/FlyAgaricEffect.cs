using System;
using System.Collections;
using UnityEngine;

public class FlyAgaricEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    public float BaseCooldown { get; private set; } = 3.6f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _flyAgaricPoisonBuff;

    private Coroutine _flyAgaricRoutine;
    private Character _targetCharacter;



    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_flyAgaricRoutine != null)
        {
            StopCoroutine(FlyAgaricRoutine());
            _flyAgaricRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_flyAgaricRoutine != null)
        {
            StopCoroutine(FlyAgaricRoutine());
            _flyAgaricRoutine = null;
        }
        CooldownMultiplier = 1f;
    }


    private IEnumerator FlyAgaricRoutine()
    {
        while (true)
        {
            _targetCharacter.ApplyBuff(_flyAgaricPoisonBuff);
            OnActivate();
            yield return new WaitForCooldown(this);
        }
    }


    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _targetCharacter = targetCharacter;

        if (_flyAgaricRoutine == null)
        {
            _flyAgaricRoutine= StartCoroutine(FlyAgaricRoutine());
        }
    }

    public void RemoveEffect()
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerFoodEffect()
    {
        _targetCharacter.ApplyBuff(_flyAgaricPoisonBuff);
        OnActivate();
    }
 
}
