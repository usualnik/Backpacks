using System;
using UnityEngine;

public class PiggyBankEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField] private int _shopEnteredMoneyAmount = 1;

    private ItemBehaviour _piggyBank;

    private void Awake()
    {
        _piggyBank = GetComponent<ItemBehaviour>();
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
        if(_piggyBank.GetItemState() == ItemBehaviour.ItemState.Inventory)
            PlayerCharacter.Instance.AddMoney(_shopEnteredMoneyAmount);
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
      
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}

