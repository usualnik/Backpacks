using System;
using System.Collections;
using UnityEngine;

public class PineappleEffect : MonoBehaviour, IItemEffect, IFoodEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _cooldown = 3.3f;
    [SerializeField] private Buff _thornsBuff;
    [SerializeField] private float _healAmount = 4f;


    private float _cooldownMultiplier = 1f;

    private Coroutine _procBuffRoutine;

    private ItemBehaviour _pineapple;

    private void Awake()
    {
        _pineapple = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_procBuffRoutine != null)
        {
            StopCoroutine(_procBuffRoutine);
            _procBuffRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_procBuffRoutine != null)
        {
            StopCoroutine(_procBuffRoutine);
            _procBuffRoutine = null;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_pineapple == null || _pineapple.OwnerCharacter == null) return;


        if (_procBuffRoutine == null)
        {
            _procBuffRoutine = StartCoroutine(ConvertHpRoutine());
        }

    }
    private IEnumerator ConvertHpRoutine()
    {
        while (true)
        {

            _pineapple.OwnerCharacter.ApplyBuff(_thornsBuff);
            _pineapple.OwnerCharacter.AddHealth(_healAmount);

            float currentCooldown = _cooldown / _cooldownMultiplier;

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
        _pineapple.OwnerCharacter.ApplyBuff(_thornsBuff);
        _pineapple.OwnerCharacter.AddHealth(_healAmount);
    }

    public void IncreaseFoodSpeed(float speedIncrease)
    {
        _cooldownMultiplier += speedIncrease;

        if (_procBuffRoutine != null)
        {
            StopCoroutine(_procBuffRoutine);
            _procBuffRoutine = StartCoroutine(ConvertHpRoutine());
        }
    }
}
