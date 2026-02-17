using System;
using UnityEngine;

public class LuckyPiggyEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField] private int _shopEnteredMoneyAmount = 1;
    [SerializeField] private Buff _luckBuff;

    private ItemBehaviour _luckyPiggy;

    private void Awake()
    {
        _luckyPiggy = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        Shop.Instance.OnShopEnteredAfterCombat += Shop_OnShopEnteredAfterCombat;
    }
    private void OnDestroy()
    {
        Shop.Instance.OnShopEnteredAfterCombat -= Shop_OnShopEnteredAfterCombat;

    }
    private void Shop_OnShopEnteredAfterCombat()
    {
        if (_luckyPiggy.GetItemState() == ItemBehaviour.ItemState.Inventory)
            PlayerCharacter.Instance.AddMoney(_shopEnteredMoneyAmount);
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_luckyPiggy.OwnerCharacter == null) return;

        _luckyPiggy.OwnerCharacter.ApplyBuff(_luckBuff);
        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
