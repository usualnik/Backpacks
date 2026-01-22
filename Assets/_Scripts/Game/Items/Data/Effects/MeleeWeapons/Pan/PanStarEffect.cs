using System.Collections.Generic;
using UnityEngine;

public class PanStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _starredFood = new List<ItemBehaviour>();

    private WeaponBehaviour _pan;

    private void Awake()
    {
        _pan = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
    }

    private void CombatManager_OnCombatStarted()
    {
        if (_starredFood.Count > 0)
            BuffDamage();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredFood.Contains(targetItem))
        {
            _starredFood.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

        if (_starredFood.Contains(targetItem))
        {
            _starredFood.Remove(targetItem);
        }
    }

    private void BuffDamage()
    {
        float damageBuff = _starredFood.Count;
        _pan.AddDamageToWeapon(damageBuff);
    }
}
