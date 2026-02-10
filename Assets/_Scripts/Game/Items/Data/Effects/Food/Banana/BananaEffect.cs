using System;
using System.Collections;
using UnityEngine;

public class BananaEffect : MonoBehaviour, IItemEffect, IFoodEffect
{
    public event Action OnEffectAcivate;

    public int ItemActivations { get; set; }

    [SerializeField] private float _healAmount = 4f;
    [SerializeField] private float _bananaEffectCooldown = 5f;
    [SerializeField] private float _regenStaminaAmount = 1f;

    private Character _targetCharacter;

    private float _currentCooldownMultiplier = 1f;

    private Coroutine _bananaRoutine;

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_bananaRoutine != null)
        {
            StopCoroutine(BananaRoutine());
            _bananaRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_bananaRoutine != null)
        {
            StopCoroutine(BananaRoutine());
            _bananaRoutine = null;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _targetCharacter = targetCharacter;

        if (_bananaRoutine == null)
        {
            _bananaRoutine = StartCoroutine(BananaRoutine());
        }
    }

    private IEnumerator BananaRoutine()
    {
        while (true)
        {
            _targetCharacter.AddHealth(_healAmount);
            _targetCharacter.AddStamina(_regenStaminaAmount);

            float currentCooldown = _bananaEffectCooldown / _currentCooldownMultiplier;

            OnActivate();

            yield return new WaitForSeconds(currentCooldown);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerEffect()
    {
        _targetCharacter.AddHealth(_healAmount);
        _targetCharacter.AddStamina(_regenStaminaAmount);
        OnActivate();
    }

    public void IncreaseFoodSpeed(float _speedIncrease)
    {
        _currentCooldownMultiplier += _speedIncrease;

        if (_bananaRoutine != null)
        {
            StopCoroutine(_bananaRoutine);
            _bananaRoutine = StartCoroutine(BananaRoutine());
        }
    }
}
