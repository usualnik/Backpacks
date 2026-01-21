using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodyDaggerStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _trackedItems = new List<ItemBehaviour>();

    private WeaponBehaviour _bloodyDagger;

    private void Awake()
    {
        _bloodyDagger = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;

    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_bloodyDagger == weapon)
        {
            ApplyDaggerEffect();
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

    private void ApplyDaggerEffect()
    {
        if (_trackedItems.Count == 0) return;

        List<ItemBehaviour> itemsWithVampirism = 
            _trackedItems.Where(i => i.ItemData.ItemExtraType == ItemDataSO.ExtraType.Vampiric).ToList();

        if (_bloodyDagger.OwnerCharacter != null && itemsWithVampirism.Count > 0)
        {
            _bloodyDagger.OwnerCharacter.AddHealth(itemsWithVampirism.Count);
        }       
       
    }
}
