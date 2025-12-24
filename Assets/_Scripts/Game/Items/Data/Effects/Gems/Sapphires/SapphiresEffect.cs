using Unity.VisualScripting;
using UnityEngine;

public class SapphiresEffect : MonoBehaviour, IItemEffect
{
    [Header("Weapon Effects")]
    [SerializeField] private float _ignoreArmorChance = 15f;
    [SerializeField] private Buff _gainedManaBuff;
    [SerializeField] private Buff _inflictedColdDebuff;

    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]  
    [SerializeField] private float _inflictColdTimer;
    [SerializeField] private Buff _bagColdDebuff;
    private bool _shouldInflictCold;
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

        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }

    private void RemoveWeaponEffect()
    {
        _gemedWeapon.OwnerCharacter.AddIgnoreArmorChance(-_ignoreArmorChance);

        _gemedWeapon = null;
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour arg1, string arg2)
    {
        if (_gemedWeapon == arg1)
        {
            TryInflictCold();
            TrygainMana();
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
        _shouldInflictCold = true;
    }

    private void CombatManager_OnCombatFinishedWithGemmedBag(CombatManager.CombatResult obj)
    {
        _shouldInflictCold = false;
    }

    private void Update()
    {
        if (!_shouldInflictCold) return;

        _inflictColdTimer -= Time.deltaTime;

        if (_inflictColdTimer <= 0)
        {
            _bagItem.TargetCharacter.ApplyBuff(_bagColdDebuff);
           _shouldInflictCold = false;
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

    private void SourceCharacter_OnNewBuffApplied(Buff.BuffType appliedBuff, bool arg2)
    {
        if (appliedBuff == Buff.BuffType.Mana)
        {
            _manaBuffGained++;

            if (_manaBuffGained == _manaBuffneeded)
            {
                _armorOrOtherItem?.OwnerCharacter?.ChangeArmorValue(_gainArmorAmount);
                _manaBuffGained = 0;
            }
        }
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


