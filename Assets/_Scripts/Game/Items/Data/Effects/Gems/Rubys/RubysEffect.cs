
using UnityEngine;

public class RubysEffect : MonoBehaviour, IItemEffect
{
    [Header("Weapon Effects")]
    [SerializeField] private float _gainedLifeStealAmount = 0.7f;
    private float _gainedAmount = 0;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _effectDamageAmount = 4;
    [SerializeField] private float _additionalLifesteal = 1.5f;
    [SerializeField] private float _dealDamageTimer = 5f;
    private ItemBehaviour _bagItem;


    private bool _shouldDealDamage = false;

    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _increaseHealingMultiplier = 0.1f;
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
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }



    private void OnDestroy()
    {
        _draggableGem.OnGemPlacedInItem -= DraggableGem_OnGemPlacedInItem;
        _draggableGem.OnGemRemovedFromItem -= DraggableGem_OnGemRemovedFromItem;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ResetLifestealAfterFight();
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
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }

    private void RemoveWeaponEffect()
    {
        _gemedWeapon = null;
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour arg1, string arg2)
    {
        if (_gemedWeapon == arg1)
        {
            GainLifesteal();
        }
    }
    private void GainLifesteal()
    {
        _gainedAmount += _gainedLifeStealAmount;
        _gemedWeapon?.SourceCharacter?.AddLifestealMultiplier(_gainedLifeStealAmount);
    }

    private void ResetLifestealAfterFight()
    {
        _gemedWeapon?.SourceCharacter?.AddLifestealMultiplier(-_gainedAmount);
    }

    #endregion

    #region Bags
    private void ApplyBagEffect(ItemBehaviour bagItem)
    {
        _bagItem = bagItem;
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStartedWithGemmedBag;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinishedWithGemmedBag;
    }


    private void RemoveBagEffect()
    {
        _bagItem = null;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStartedWithGemmedBag;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinishedWithGemmedBag;
    }


    private void CombatManager_OnCombatStartedWithGemmedBag()
    {
        _shouldDealDamage = true;
    }

    private void CombatManager_OnCombatFinishedWithGemmedBag(CombatManager.CombatResult obj)
    {
        _shouldDealDamage = false;
        _bagItem?.SourceCharacter?.AddLifestealMultiplier(-_additionalLifesteal);
    }

    private void Update()
    {
        if (!_shouldDealDamage) return;

        _dealDamageTimer -= Time.deltaTime;

        if (_dealDamageTimer <= 0)
        {
            _bagItem?.TargetCharacter?.TakeDamage(_effectDamageAmount, ItemDataSO.ExtraType.Effect);
            _bagItem?.SourceCharacter?.AddLifestealMultiplier(_additionalLifesteal);
           _shouldDealDamage = false;
        }

    }


    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem.SourceCharacter.AddHealthRegenMultiplier(_increaseHealingMultiplier);
    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem.SourceCharacter.AddHealthRegenMultiplier(-_increaseHealingMultiplier);
        _armorOrOtherItem = null;

    }
    #endregion


    #region Interface
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
    }

    public void RemoveEffect()
    {

    }
    #endregion

}


