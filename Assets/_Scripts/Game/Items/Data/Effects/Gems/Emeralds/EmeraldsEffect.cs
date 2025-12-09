using System.Collections;
using UnityEngine;

public class EmeraldsEffect : MonoBehaviour, IItemEffect
{
    [Header("Weapon Effects")]
    [SerializeField] private float _chanceToInfictPoison = 35;
    [SerializeField] private Buff _emeraldWeaponPoisonBuff;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _gainRegenerationBuffTimer;
    [SerializeField] private Buff _regenerationBuff;
    private bool _isShouldBuffRegen = false;
    private ItemBehaviour _bagItem;

    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _increaseResistPoisonChance = 0.1f;
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
            TryToInflictPoison();
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
        _isShouldBuffRegen = true;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _isShouldBuffRegen = false;
    }

    private void Update()
    {
        if (!_isShouldBuffRegen) return;

        _gainRegenerationBuffTimer -= Time.deltaTime;

        if (_gainRegenerationBuffTimer <= 0)
        {
            _bagItem.SourceCharacter.ApplyBuff(_regenerationBuff);
            _isShouldBuffRegen = false;
        }

    }


    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem?.SourceCharacter?.AddPoisonResistChance(_increaseResistPoisonChance);
    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem?.SourceCharacter?.AddPoisonResistChance(-_increaseResistPoisonChance);
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

