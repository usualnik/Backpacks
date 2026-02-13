
using System;
using System.Collections;
using UnityEngine;

public class RubysEffect : MonoBehaviour, IItemEffect, ICooldownable
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    public float BaseCooldown { get; private set; }
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [Header("Weapon Effects")]
    [SerializeField] private float _gainedLifeStealAmount = 0.7f;
    private float _gainedAmount = 0;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _effectDamageAmount = 4;
    [SerializeField] private float _additionalLifesteal = 1.5f;
    [SerializeField] private float _dealDamageTimer = 5f;
    private Coroutine _dealDamageRoutine;

    private ItemBehaviour _bagItem;



    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _increaseHealingMultiplier = 0.1f;
    private ItemBehaviour _armorOrOtherItem;


    private DraggableGem _draggableGem;

  

    private void Awake()
    {
        _draggableGem = GetComponent<DraggableGem>();

        BaseCooldown = _dealDamageTimer;

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
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
    }

    private void RemoveWeaponEffect()
    {
        _gemedWeapon = null;
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour arg1, Character character, float damage)
    {
        if (_gemedWeapon == arg1)
        {
            GainLifesteal();
            OnActivate();
        }
    }
    private void GainLifesteal()
    {
        _gainedAmount += _gainedLifeStealAmount;
        _gemedWeapon?.OwnerCharacter?.AddLifestealMultiplier(_gainedLifeStealAmount);
    }

    private void ResetLifestealAfterFight()
    {
        _gemedWeapon?.OwnerCharacter?.AddLifestealMultiplier(-_gainedAmount);
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


        if(_dealDamageRoutine ==  null)
        {
            _dealDamageRoutine = StartCoroutine(DealDamageRoutine());
        }

    }

    private void CombatManager_OnCombatFinishedWithGemmedBag(CombatManager.CombatResult obj)
    {
        _bagItem?.OwnerCharacter?.AddLifestealMultiplier(-_additionalLifesteal);


        if(_dealDamageRoutine != null)
        {
            StopCoroutine(_dealDamageRoutine);
            _dealDamageRoutine = null;
        }
        CooldownMultiplier = 1f;
    }


    private IEnumerator DealDamageRoutine()
    {
        while (true)
        {
            _bagItem?.TargetCharacter?.TakeDamage(_effectDamageAmount, ItemDataSO.ExtraType.Effect);
            _bagItem?.OwnerCharacter?.AddLifestealMultiplier(_additionalLifesteal);

            OnActivate();

            yield return new WaitForCooldown(this);
        }
    }


    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem.OwnerCharacter.AddHealthRegenMultiplier(_increaseHealingMultiplier);
    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem.OwnerCharacter.AddHealthRegenMultiplier(-_increaseHealingMultiplier);
        _armorOrOtherItem = null;

    }
    #endregion


    #region Interface
    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
    }

    public void RemoveEffect()
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
    #endregion

}


