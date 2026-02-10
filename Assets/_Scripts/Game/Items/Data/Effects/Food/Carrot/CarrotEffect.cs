using System;
using System.Collections;
using UnityEngine;

public class CarrotEffect : MonoBehaviour, IItemEffect, IFoodEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _empowerrBuff;
    [SerializeField] private float _cooldown = 2.7f;
    [SerializeField] private int _cleanseDebuffAmount = 1;
    [SerializeField] private int _luckMinimumNeededToProcEmpower = 4;
    [SerializeField] private int _chanceToProcEmpower = 55;

    private float _cooldownMultiplier = 1f;

    private ItemBehaviour _carrot;
    private Coroutine _carrotRoutine;

    private void Awake()
    {
        _carrot = GetComponent<ItemBehaviour>();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_carrotRoutine != null)
        {
            StopCoroutine(CarrotRoutine());
            _carrotRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_carrotRoutine != null)
        {
            StopCoroutine(CarrotRoutine());
            _carrotRoutine = null;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_carrot.OwnerCharacter == null) return;

        if (_carrotRoutine == null)
        {
            _carrotRoutine = StartCoroutine(CarrotRoutine());
        }
    }

    private IEnumerator CarrotRoutine()
    {
        while (true)
        {

            if (_carrot.OwnerCharacter)
            {
                _carrot.OwnerCharacter.RemoveBuff(Buff.GetRandomDebuffType(), 1);


                if (_carrot.OwnerCharacter.GetBuffStacks(Buff.BuffType.Luck)
                    > _luckMinimumNeededToProcEmpower)
                {
                    TryProcEmpower();
                }

                OnActivate();
            }


            float currentCooldown = _cooldown / _cooldownMultiplier;


            yield return new WaitForSeconds(currentCooldown);
        }
    }

    private void TryProcEmpower()
    {
        bool isProc = UnityEngine.Random.Range(0, 101) < _chanceToProcEmpower;

        if (isProc)
        {
            _carrot.OwnerCharacter.ApplyBuff(_empowerrBuff);
        }
    }

    public void TriggerEffect()
    {
        if (_carrot.OwnerCharacter)
        {
            _carrot.OwnerCharacter.RemoveBuff(Buff.GetRandomDebuffType(), 1);


            if (_carrot.OwnerCharacter.GetBuffStacks(Buff.BuffType.Luck)
                > _luckMinimumNeededToProcEmpower)
            {
                TryProcEmpower();
            }
        }
    }

    public void IncreaseFoodSpeed(float speedIncrease)
    {
        _cooldownMultiplier += speedIncrease;

        if (_carrotRoutine != null)
        {
            StopCoroutine(_carrotRoutine);
            _carrotRoutine = StartCoroutine(CarrotRoutine());
        }
    }
}
