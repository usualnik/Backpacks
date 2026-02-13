using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyArmorStarEffect : MonoBehaviour, ICooldownable
{
    [SerializeField] private int _armorBuff = 65;
    [SerializeField] private Buff _regenBuff;

    public float BaseCooldown { get; private set; } = 2.2f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private int _cleansePoisonAmount = 2;

    private List<ItemBehaviour> _itemsInStar = new List<ItemBehaviour>();

    private ItemBehaviour _holyArmor;
    private Coroutine _cleansePoisonRoutine;

    private void Awake()
    {
        _holyArmor = GetComponent<ItemBehaviour>();
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

        if (_cleansePoisonRoutine != null)
        {
            StopCoroutine(_cleansePoisonRoutine);
            _cleansePoisonRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_cleansePoisonRoutine != null)
        {
            StopCoroutine(_cleansePoisonRoutine);
            _cleansePoisonRoutine = null;
        }

        CooldownMultiplier = 1f;
    }


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_itemsInStar.Contains(targetItem) && targetItem.ItemData.ItemExtraType == ItemDataSO.ExtraType.Holy)
        {
            _itemsInStar.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_itemsInStar.Contains(targetItem)
            && targetItem.ItemData.ItemExtraType == ItemDataSO.ExtraType.Holy)
        {
            _itemsInStar.Remove(targetItem);
        }
    }
    private void CombatManager_OnCombatStarted()
    {
        ApplyHolyArmorBuff();

        if (_cleansePoisonRoutine == null)
        {
            _cleansePoisonRoutine = StartCoroutine(CleansePoisonCorutine());
        }
    }

    private void ApplyHolyArmorBuff()
    {
        if (_holyArmor == null || _holyArmor.OwnerCharacter == null) return;

        _holyArmor.OwnerCharacter.AddArmor(_armorBuff);

        foreach (var item in _itemsInStar)
            _holyArmor.OwnerCharacter.ApplyBuff(_regenBuff);
    }

    private IEnumerator CleansePoisonCorutine()
    {
        while (true)
        {
            _holyArmor.OwnerCharacter.RemoveBuff(Buff.BuffType.Poison,_cleansePoisonAmount);

            yield return new WaitForCooldown(this);
        }        
    }
}
