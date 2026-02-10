using System.Collections.Generic;
using UnityEngine;

public class VampiricGlovesStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _speedIncrease = 0.35f;
    [SerializeField] private Buff _vampBuff;

    private List<ItemBehaviour> _itemsInStar = new List<ItemBehaviour>();

    private ItemBehaviour _vampiricGloves;

    private void Awake()
    {
        _vampiricGloves = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
    }



    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Remove(targetItem);
        }
    }

    private void CombatManager_OnCombatStarted()
    {
        Invoke(nameof(ProcBuff), 4f);
    }

    private void ProcBuff()
    {
        foreach (ItemBehaviour sourceItem in _itemsInStar)
        {
            sourceItem.TryGetComponent(out WeaponBehaviour weapon);
            if (weapon != null)
            {
                weapon.IncreaseSpeedMultiplier(_speedIncrease);
            }
        }

        if (_vampiricGloves == null || _vampiricGloves.OwnerCharacter == null) return;

        _vampiricGloves.OwnerCharacter.ApplyBuff(_vampBuff);

    }
}
