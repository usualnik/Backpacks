using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class EmeraldsEffect : MonoBehaviour, IItemEffect, ICooldownable
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
    [SerializeField] private float _chanceToInfictPoison = 35;
    [SerializeField] private Buff _emeraldWeaponPoisonBuff;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _gainRegenerationBuffTimer;
    [SerializeField] private Buff _regenerationBuff;
    private Coroutine _gainRegenRoutine;
    private ItemBehaviour _bagItem;

    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _increaseResistPoisonChance = 10f;
    private ItemBehaviour _armorOrOtherItem;
   
    private DraggableGem _draggableGem;

  

    private void Awake()
    {
        _draggableGem = GetComponent<DraggableGem>();

        BaseCooldown = _gainRegenerationBuffTimer;

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

        if (_gainRegenRoutine != null)
        {
            StopCoroutine(GainRegenRoutine());
            _gainRegenRoutine = null;
        }

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
            TryToInflictPoison();
            OnActivate();
        }
    }

    private void TryToInflictPoison()
    {
        bool isProc = UnityEngine.Random.Range(0, 100) <= _chanceToInfictPoison ? true : false;

        if (isProc)
        {
            _gemedWeapon.TargetCharacter.ApplyBuff(_emeraldWeaponPoisonBuff);
        }
    }

    #endregion

    #region Bags
    private void ApplyBagEffect(ItemBehaviour bagItem)
    {
        _bagItem = bagItem;
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void RemoveBagEffect()
    {
        _bagItem = null;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }


    private void CombatManager_OnCombatStarted()
    {
        if(_gainRegenRoutine == null)
        {
            _gainRegenRoutine = StartCoroutine(GainRegenRoutine());
        }
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_gainRegenRoutine != null)
        {
            StopCoroutine(GainRegenRoutine());
            _gainRegenRoutine = null;
        }
        CooldownMultiplier = 1f;
    }


    private IEnumerator GainRegenRoutine()
    {
        while (true)
        {
            _bagItem.OwnerCharacter.ApplyBuff(_regenerationBuff);

            OnActivate();
            yield return new WaitForCooldown(this);
        }
    }


    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem?.OwnerCharacter?.AddPoisonResistChance(_increaseResistPoisonChance);
    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem?.OwnerCharacter?.AddPoisonResistChance(-_increaseResistPoisonChance);
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

