using System;
using System.Collections;
using UnityEngine;

public class FlyAgaricEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }



    [SerializeField] private Buff _flyAgaricPoisonBuff;
    [SerializeField] private float _flyAgaricEffectCooldown;

    private Coroutine _flyAgaricRoutine;
    private Character _targetCharacter;
    private float _currentCooldownMultiplier = 1f;



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
    }


    private IEnumerator FlyAgaricRoutine()
    {
        while (true)
        {
            _targetCharacter.ApplyBuff(_flyAgaricPoisonBuff);
            float currentCooldown = _flyAgaricEffectCooldown / _currentCooldownMultiplier;
            OnActivate();
            yield return new WaitForSeconds(currentCooldown);
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

    public void IncreaseSpeed(float percentageIncrease)
    {
        _currentCooldownMultiplier += percentageIncrease;

        if (_flyAgaricRoutine != null)
        {
            StopCoroutine(_flyAgaricRoutine);
            _flyAgaricRoutine = StartCoroutine(FlyAgaricRoutine());
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
