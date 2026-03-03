using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JynxTorquillaStarEffect : MonoBehaviour, IStarEffect, ICooldownable
{
    public float BaseCooldown => 3f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private List<ItemBehaviour> _trackedItems = new List<ItemBehaviour>();

    [SerializeField] private float _speedIncreaseBuffAmount = 0.05f;

    private const int MAX_BUFF_STACKS = 10;
    private int _buffActivations;

    private ItemBehaviour _jynx;

    private Coroutine _jynxRoutine;

    private void Awake()
    {
        _trackedItems = new List<ItemBehaviour>();
        _jynx = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_jynxRoutine != null)
        {
            StopCoroutine(_jynxRoutine);
            _jynxRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_jynxRoutine != null)
        {
            StopCoroutine(_jynxRoutine);
            _jynxRoutine = null;
        }

        _buffActivations = 0;
        CooldownMultiplier = 1f;
    }
    private void CombatManager_OnCombatStarted()
    {
        if (_jynxRoutine == null && _jynx.isActiveAndEnabled)
        {
            _jynxRoutine = StartCoroutine(JynxRoutine());
        }
    }


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

        if (!_trackedItems.Contains(targetItem))
        {
            _trackedItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_trackedItems.Contains(targetItem))
        {
            _trackedItems.Remove(targetItem);
        }
    }

    private IEnumerator JynxRoutine()
    {
        while (_buffActivations < MAX_BUFF_STACKS)
        {
            _buffActivations++;

            foreach(var item in _trackedItems)
            {
                if (item.TryGetComponent(out ICooldownable cooldownableItem))
                {
                    cooldownableItem.CooldownMultiplier += _speedIncreaseBuffAmount;
                }
            }

            yield return new WaitForCooldown(this);
        }
    }


}
