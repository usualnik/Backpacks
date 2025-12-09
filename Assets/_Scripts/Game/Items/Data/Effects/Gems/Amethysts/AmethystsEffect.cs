using System;
using System.Collections;
using UnityEngine;
using static Buff;

public class AmethystsEffect : MonoBehaviour, IItemEffect
{
    [Header("Weapon Effects")]
    [SerializeField] private float _chanceToRemoveRandomBuffFromOpponent = 25;
    [SerializeField] private int _removeBuffsAmount = 1;
    private WeaponBehaviour _gemedWeapon;

    [Header("Bag Effects")]
    [SerializeField] private float _cleanseDebuffTimer = 3.2f;
    private Coroutine _cleanseDebuffCoroutine;
    private ItemBehaviour _bagItem;

    [Header("ArmorAndOther Effects")]
    [SerializeField] private float _reduceOpponentHealingPercentage = 0.12f;
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
            TryToRemoveRandomBuffFromOpponent();
        }
    }

    private void TryToRemoveRandomBuffFromOpponent()
    {
        bool isProc = UnityEngine.Random.Range(0,100) <= _chanceToRemoveRandomBuffFromOpponent ? true : false;

        if (isProc)
        {
            BuffType[] buffTypes = (BuffType[])Enum.GetValues(typeof(BuffType));

            //HACK: Переделать одним из первых! Хардкод значений, так как пока нет разделения на бафы и дебафы, они все в одном энаме.
            int randomIndex = UnityEngine.Random.Range(1, 8);
            BuffType randomBuff = buffTypes[randomIndex];

            _gemedWeapon.TargetCharacter.RemoveBuff(randomBuff, _removeBuffsAmount);
        }
    }

    #endregion

    #region Bags
    private void ApplyBagEffect(ItemBehaviour bagItem)
    {
        _bagItem = bagItem;
        _cleanseDebuffCoroutine = StartCoroutine(CleanseDebuffCorutine());
    }
    private void RemoveBagEffect()
    {
        _bagItem = null;
        StopCoroutine(_cleanseDebuffCoroutine);
        _cleanseDebuffCoroutine = null;
    }

    private IEnumerator CleanseDebuffCorutine()
    {
        BuffType[] buffTypes = (BuffType[])Enum.GetValues(typeof(BuffType));

        //HACK: Переделать одним из первых! Хардкод значений, так как пока нет разделения на бафы и дебафы, они все в одном энаме.
        int randomIndex = UnityEngine.Random.Range(8, buffTypes.Length);
        BuffType randomBuff = buffTypes[randomIndex];

        while (true)
        {
            _bagItem?.SourceCharacter?.RemoveBuff(randomBuff, 1);
            Debug.Log("Trying to remove debuff");
            yield return new WaitForSeconds(_cleanseDebuffTimer);
        }
    }


    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect(ItemBehaviour armorOrOtherItem)
    {
        _armorOrOtherItem = armorOrOtherItem;
        _armorOrOtherItem.TargetCharacter.AddHealthRegenMultiplier(-_reduceOpponentHealingPercentage);
    }
    private void RemoveArmorOrOtherEffect()
    {
        _armorOrOtherItem.TargetCharacter.AddHealthRegenMultiplier(_reduceOpponentHealingPercentage);
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
