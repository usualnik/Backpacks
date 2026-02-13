using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SapphiresEffect : MonoBehaviour, IItemEffect, ICooldownable
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

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
    [SerializeField] private float _ignoreArmorChance = 15f;
    [SerializeField] private Buff _gainedManaBuff;
    [SerializeField] private Buff _inflictedColdDebuff;

    private Coroutine _inflictColdRoutine;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]  
    [SerializeField] private float _inflictColdTimer;
    [SerializeField] private Buff _bagColdDebuff;
    private ItemBehaviour _bagItem;


    [Header("ArmorAndOther Effects")]
    [SerializeField] private int _gainArmorAmount = 2;
    [SerializeField] private int _manaBuffneeded = 5;
    private ItemBehaviour _armorOrOtherItem;
    private int _manaBuffGained = 0;
    


    private DraggableGem _draggableGem;


    private void Awake()
    {
        _draggableGem = GetComponent<DraggableGem>();

        BaseCooldown = _inflictColdTimer;

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
        _gemedWeapon.OwnerCharacter.AddIgnoreArmorChance(_ignoreArmorChance);

        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
    }

    private void RemoveWeaponEffect()
    {
        _gemedWeapon.OwnerCharacter.AddIgnoreArmorChance(-_ignoreArmorChance);

        _gemedWeapon = null;
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour arg1, Character character, float damage)
    {
        if (_gemedWeapon == arg1)
        {
            TryInflictCold();
            TrygainMana();
            OnActivate();
        }
    }
    private void TryInflictCold()
    {
        bool isProc = UnityEngine.Random.Range(0f,100f) <= _ignoreArmorChance ? true : false;
        if (isProc)
        {
            _gemedWeapon?.TargetCharacter?.ApplyBuff(_inflictedColdDebuff);
        }
    }

    private void TrygainMana()
    {
        bool isProc = UnityEngine.Random.Range(0f, 100f) <= _ignoreArmorChance ? true : false;
        if (isProc)
        {
            _gemedWeapon?.OwnerCharacter?.ApplyBuff(_gainedManaBuff);
        }
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

        if (_inflictColdRoutine == null)
            _inflictColdRoutine = StartCoroutine(InflictColdRoutine());
    }

    private void CombatManager_OnCombatFinishedWithGemmedBag(CombatManager.CombatResult obj)
    {

        if (_inflictColdRoutine != null)
        {
            StopCoroutine(_inflictColdRoutine);
            _inflictColdRoutine = null;
        }

        CooldownMultiplier = 1f;
    }


    private IEnumerator InflictColdRoutine()
    {
        while (true)
        {
            _bagItem.TargetCharacter.ApplyBuff(_bagColdDebuff);
            OnActivate();
            yield return new WaitForCooldown(this);
        }
    }



    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem.OwnerCharacter.OnNewBuffApplied += SourceCharacter_OnNewBuffApplied;
    }

   

    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem.OwnerCharacter.OnNewBuffApplied -= SourceCharacter_OnNewBuffApplied;
        _armorOrOtherItem = null;
    }

    private void SourceCharacter_OnNewBuffApplied(Buff buff)
    {
        if (buff.Type == Buff.BuffType.Mana)
        {
            _manaBuffGained++;

            if (_manaBuffGained == _manaBuffneeded)
            {
                _armorOrOtherItem?.OwnerCharacter?.AddArmor(_gainArmorAmount);
                _manaBuffGained = 0;
            }
        }
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


