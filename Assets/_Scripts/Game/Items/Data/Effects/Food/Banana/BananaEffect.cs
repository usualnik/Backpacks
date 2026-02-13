using System;
using System.Collections;
using UnityEngine;

public class BananaEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public event Action OnEffectAcivate;

    public int ItemActivations { get; set; }

    public float BaseCooldown { get; private set; } = 5f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private float _healAmount = 4f;
    [SerializeField] private float _regenStaminaAmount = 1f;

    private Character _targetCharacter;
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

        CooldownMultiplier = 1f;
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

            OnActivate();

            yield return new WaitForCooldown(this);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerFoodEffect()
    {
        _targetCharacter.AddHealth(_healAmount);
        _targetCharacter.AddStamina(_regenStaminaAmount);
        OnActivate();
    }

}
