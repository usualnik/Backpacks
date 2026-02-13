using System;
using System.Collections;
using UnityEngine;

public class PineappleEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 3.3f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _thornsBuff;
    [SerializeField] private float _healAmount = 4f;

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
        _pineapple.OwnerCharacter.ApplyBuff(_thornsBuff);
        _pineapple.OwnerCharacter.AddHealth(_healAmount);
    }
   
}
