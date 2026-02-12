using System;
using System.Collections.Generic;
using UnityEngine;

public class StorageCoffinEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }


    private Bag _bag;

    private float _chanceToEnflictPoison = 25f;

    private List<ItemBehaviour> _itemsInCoffin;

    private Buff _storageCoffinBuff;
    private Character _targetCharacter;


    private void Awake()
    {
        _bag = GetComponent<Bag>();
        _storageCoffinBuff = new Buff
        {
            Name = "StorageCoffinBuff",
            Type = Buff.BuffType.Poison,
            IsPositive = false,
            Value = 1

        };
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        foreach (var item in _itemsInCoffin)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate += ItemInCoffin_OnItemActionPerformed;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _itemsInCoffin = _bag.ItemsInbag;

        foreach (var i in _itemsInCoffin)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate -= ItemInCoffin_OnItemActionPerformed;
        }
    }

    private void ItemInCoffin_OnItemActionPerformed()
    {
        bool isProc = UnityEngine.Random.Range(0f, 100f) <= _chanceToEnflictPoison ? true : false;

        _targetCharacter = GetComponent<ItemBehaviour>().TargetCharacter;
        
        if (isProc)
        {
            _targetCharacter.ApplyBuff(_storageCoffinBuff);
        }
    }

    public void RemoveEffect()
    {

    }

    public void OnActivate()
    {

    }
}
