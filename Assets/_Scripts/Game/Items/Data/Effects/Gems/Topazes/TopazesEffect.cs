using System;
using UnityEngine;

public class TopazesEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [Header("Weapon Effects")]
    [SerializeField] private float _increaseSpeedMultiplier = 0.1f;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _gainStaminaRegenValue = 0.08f;
    private ItemBehaviour _bagItem;


    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _gainChanceToResistStun = 10f;
    [SerializeField] private float _gainChanceToCriticalHit = 5f;

    private ItemBehaviour _armorOrOtherItem;
   

    private DraggableGem _draggableGem;



    private void Awake()
    {
        _draggableGem = GetComponent<DraggableGem>();

    }
    private void Start()
    {
        _draggableGem.OnGemPlacedInItem += DraggableGem_OnGemPlacedInItem;
        _draggableGem.OnGemRemovedFromItem += DraggableGem_OnGemRemovedFromItem;
    }


    private void OnDestroy()
    {
        _draggableGem.OnGemPlacedInItem -= DraggableGem_OnGemPlacedInItem;
        _draggableGem.OnGemRemovedFromItem -= DraggableGem_OnGemRemovedFromItem;

    }
    private void DraggableGem_OnGemRemovedFromItem(ItemBehaviour itemWithSocket)
    {
        ItemDataSO itemData = itemWithSocket.ItemData;

        switch (itemData.Type)
        {
            case ItemDataSO.ItemType.Weapon:
                RemoveWeaponEffect();
                break;           
            case ItemDataSO.ItemType.Bag:
                RemoveBagEffect();
                break;
            default:
                RemoveArmorOrOtherEffect();
                break;
        }
    }

    private void DraggableGem_OnGemPlacedInItem(ItemBehaviour itemWithSocket)
    {
        ItemDataSO itemData = itemWithSocket.ItemData;

        WeaponBehaviour weaponBehaviour = null;

        if (itemWithSocket is WeaponBehaviour)
        {
            weaponBehaviour = itemWithSocket as WeaponBehaviour;
        }

        switch (itemData.Type)
        {
            case ItemDataSO.ItemType.Weapon:
                ApplyWeaponEffect(weaponBehaviour);
                break;          
            case ItemDataSO.ItemType.Bag:
                ApplyBagEffect(itemWithSocket);
                break;
            default:
                ApplyArmorOrOtherEffect(itemWithSocket);
                break;
        }
    }

    #region Weapons
    private void ApplyWeaponEffect(WeaponBehaviour weaponBehaviour)
    {
        if (weaponBehaviour == null) return;
        _gemedWeapon = weaponBehaviour;
        _gemedWeapon.CooldownMultiplier += _increaseSpeedMultiplier;

    }

    private void RemoveWeaponEffect()
    {
        _gemedWeapon.CooldownMultiplier -= _increaseSpeedMultiplier;
        _gemedWeapon = null;
    }

    #endregion

    #region Bags
    private void ApplyBagEffect(ItemBehaviour bagItem)
    {
        _bagItem = bagItem;
        _bagItem.OwnerCharacter.AddStaminaRegenStepMultiplier(_gainStaminaRegenValue);       
    }


    private void RemoveBagEffect()
    {
        _bagItem.OwnerCharacter.AddStaminaRegenStepMultiplier(-_gainStaminaRegenValue);
        _bagItem = null;
      
    }

    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem?.OwnerCharacter?.AddStunResistChance(_gainChanceToResistStun);
        _armorOrOtherItem?.OwnerCharacter?.AddCriticalHitResistChance(_gainChanceToCriticalHit);

    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem?.OwnerCharacter?.AddStunResistChance(-_gainChanceToResistStun);
        _armorOrOtherItem?.OwnerCharacter?.AddCriticalHitResistChance(-_gainChanceToCriticalHit);

        _armorOrOtherItem = null;
    }
    #endregion

    #region Interface
    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
    }     

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
    #endregion

}



