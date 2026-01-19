using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RainbowGoobertStarEffect : MonoBehaviour
{
    [SerializeField] private List<ItemBehaviour> _trackedItems;
    [SerializeField] private float _healAmount = 20f;
    [SerializeField] private float _armorAmount = 20f;
    [SerializeField] private float _additionalWeaponDamage = 4f;

    [SerializeField] private Buff _vampirismBuff;
    [SerializeField] private Buff _blindDebuff;
    [SerializeField] private Buff _poisonDebuff;



    private const int ITEM_ACTIVATIONS_NEEDED_TO_PROC = 5;
    private int _itemsActivations;

    private Character _ownerCharacter;
    private Character _targetCharacter;

    private List<WeaponBehaviour> _buffedWeapons;


    private void Awake()
    {
        _trackedItems = new List<ItemBehaviour>();
        _buffedWeapons = new List<WeaponBehaviour>();
    }

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
        _targetCharacter = GetComponent<ItemBehaviour>().TargetCharacter;

        if (_ownerCharacter != null && _targetCharacter != null)
        {
            WeaponsDamageBuff();

            _ownerCharacter.AddHealth(_healAmount);
            _ownerCharacter.AddArmor(_armorAmount);
            _ownerCharacter.ApplyBuff(_vampirismBuff);

            _targetCharacter.ApplyBuff(_poisonDebuff);
            _targetCharacter.ApplyBuff(_blindDebuff);
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

