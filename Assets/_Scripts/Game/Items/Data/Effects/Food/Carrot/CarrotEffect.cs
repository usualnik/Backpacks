using System;
using System.Collections;
using UnityEngine;

public class CarrotEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 2.7f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _empowerrBuff;
    [SerializeField] private int _luckMinimumNeededToProcEmpower = 4;
    [SerializeField] private int _chanceToProcEmpower = 55;


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

        CooldownMultiplier = 1f;
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




            yield return new WaitForCooldown(this);
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

    public void TriggerFoodEffect()
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

}
