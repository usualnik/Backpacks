using System;
using System.Collections.Generic;
using UnityEngine;

public class FannyPackEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    [SerializeField] private float _speedIncrease = 0.1f;

    private Bag _bag;

    private List<ItemBehaviour> _itemsInFannyPack;


    private void Awake()
    {
        _bag = GetComponent<Bag>();
    } 

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _itemsInFannyPack = _bag.ItemsInbag;

        foreach (var i in _itemsInFannyPack)
        {
            if(i.TryGetComponent(out ICooldownable cooldownableItem))
            {
                cooldownableItem.CooldownMultiplier += _speedIncrease;                
            }
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
