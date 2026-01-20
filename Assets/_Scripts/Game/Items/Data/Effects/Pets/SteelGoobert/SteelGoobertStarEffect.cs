using System.Collections.Generic;
using UnityEngine;

public class SteelGoobertStarEffect : MonoBehaviour
{
    [SerializeField] private List<ItemBehaviour> _trackedItems = new List<ItemBehaviour>();

    [SerializeField] private float _additionalWeaponDamage = 2f;
    [SerializeField] private float _armorBuffAmount = 16f;

    private const int ITEM_ACTIVATIONS_NEEDED_TO_PROC = 5;
    private int _itemsActivations;

    private Character _ownerCharacter;

    private List<WeaponBehaviour> _buffedWeapons = new List<WeaponBehaviour>();
 

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;


    }
    private void CombatManager_OnCombatStarted()
    {
        TrackStaredItems();
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopTrackItems();
        RemoveWeaponsDamageBuffAfterCombat();
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

    private void TrackStaredItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate += GoobertStarEffect_OnEffectAcivate;
        }
    }

    private void StopTrackItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate -= GoobertStarEffect_OnEffectAcivate;
        }

        _itemsActivations = 0;
    }

    private void GoobertStarEffect_OnEffectAcivate()
    {
        CountActivations();
    }

    private void CountActivations()
    {
        _itemsActivations++;

        if (_itemsActivations >= ITEM_ACTIVATIONS_NEEDED_TO_PROC)
        {
            GoobertEffect();
            _itemsActivations = 0;
        }
    }

    private void GoobertEffect()
    {
        _ownerCharacter = GetComponent<ItemBehaviour>().OwnerCharacter;

        if (_ownerCharacter != null)
        {
            WeaponsDamageBuff();

            _ownerCharacter.AddArmor(_armorBuffAmount);

        }
    }


    private void WeaponsDamageBuff()
    {

        foreach (ItemBehaviour item in _trackedItems)
        {
            if (item.ItemData.Type == ItemDataSO.ItemType.Weapon)
            {
                _buffedWeapons.Add(item as WeaponBehaviour);
            }
        }

        foreach (WeaponBehaviour weapon in _buffedWeapons)
        {
            weapon.AddDamageToWeapon(_additionalWeaponDamage);
        }
    }

    private void RemoveWeaponsDamageBuffAfterCombat()
    {
        foreach (WeaponBehaviour weapon in _buffedWeapons)
        {
            weapon.AddDamageToWeapon(-_additionalWeaponDamage);
        }

        _buffedWeapons.Clear();
    }

}

