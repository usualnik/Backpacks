using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StaffOfUnhealingEffect : MonoBehaviour, IItemEffect, ICooldownable
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 2f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private float _healingAmount = 20;
    [SerializeField] private int _manaToProcNeeded = 5;

    private WeaponBehaviour _staff;
    private Coroutine _staffRoutine;

    private void Awake()
    {
        _staff = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_staffRoutine != null)
        {
            StopCoroutine(_staffRoutine);
            _staffRoutine = null;
        }

        _staff.OwnerCharacter.OnHealingRecived -= OwnerCharacter_OnHealingRecived;

        CooldownMultiplier = 1f;

    }

    private void OwnerCharacter_OnHealingRecived(float healingAmount)
    {
        ConvertHealingToDamage(healingAmount);
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_staffRoutine == null)
        {
            _staffRoutine = StartCoroutine(StaffRoutine());
        }
    }

    private IEnumerator StaffRoutine()
    {
        while (true)
        {
            _staff.OwnerCharacter.AddHealth(_healingAmount);

            if (_staff.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) >= _manaToProcNeeded)
            {
                StartHealingToDamageProc();
            }

            OnActivate();

            yield return new WaitForCooldown(this);
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void StartHealingToDamageProc()
    {
        if (CombatManager.Instance.IsInCombat)
        {
            _staff.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, _manaToProcNeeded);
            _staff.OwnerCharacter.OnHealingRecived += OwnerCharacter_OnHealingRecived;
            Invoke(nameof(StopConvertHealinToDamage), 2);
        }
    }
  
    private void StopConvertHealinToDamage()
    {
        if (CombatManager.Instance.IsInCombat)
        {
            _staff.OwnerCharacter.OnHealingRecived -= OwnerCharacter_OnHealingRecived;
        }
    }    

    private void ConvertHealingToDamage(float healingAmount)
    {
        _staff.TargetCharacter.TakeDamage(healingAmount, ItemDataSO.ExtraType.Effect);
    }

}
