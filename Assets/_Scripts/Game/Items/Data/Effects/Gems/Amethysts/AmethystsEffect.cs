using System;
using System.Collections;
using UnityEngine;
using static Buff;

public class AmethystsEffect : MonoBehaviour, IItemEffect, ICooldownable
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

        BaseCooldown = _cleanseDebuffTimer;

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


        if (_cleanseDebuffCoroutine != null)
        {
            StopCoroutine(_cleanseDebuffCoroutine);
            _cleanseDebuffCoroutine = null;
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
            TryToRemoveRandomBuffFromOpponent();
            OnActivate();
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

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_cleanseDebuffCoroutine != null)
        {
            StopCoroutine(_cleanseDebuffCoroutine);
            _cleanseDebuffCoroutine = null;
        }

        CooldownMultiplier = 1f;
    }

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
        while (true)
        {
            _bagItem?.OwnerCharacter?.RemoveBuff(Buff.GetRandomBuffType(), 1);
            Debug.Log("Trying to remove debuff");
            OnActivate();
            yield return new WaitForCooldown(this);
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
