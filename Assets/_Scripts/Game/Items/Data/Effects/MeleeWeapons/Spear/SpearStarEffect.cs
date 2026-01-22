using System.Collections.Generic;
using UnityEngine;

public class SpearStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _starredItems = new List<ItemBehaviour>();
    [SerializeField] private StarCell[] _starCells;
    [SerializeField] private int _destroyArmorAmount = 4;

    private WeaponBehaviour _spear;

    private void Awake()
    {
        _spear = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;

    }

    
    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_spear != weapon) return;
        DestroyArmorOnHit();
    }


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredItems.Contains(targetItem))
        {
            _starredItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

        if (_starredItems.Contains(targetItem))
        {
            _starredItems.Remove(targetItem);
        }
    }

    private void DestroyArmorOnHit()
    {
        if (_spear.TargetCharacter == null) return;

        int emptyStarCells = _starCells.Length;

        foreach (var starCell in _starCells)
        {
            if (starCell.IsFilled)
            {
                emptyStarCells--;
            }
        }

        if (emptyStarCells > 0)
        {
            for (int i = 0; i < emptyStarCells; i++)
            {
                _spear.TargetCharacter.AddArmor(-_destroyArmorAmount);
            }
        }
    }

}
