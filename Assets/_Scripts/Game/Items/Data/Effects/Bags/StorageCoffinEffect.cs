using System.Collections.Generic;
using UnityEngine;

public class StorageCoffinEffect : MonoBehaviour, IItemEffect
{
    private Bag _bag;

    private float _chanceToEnflictPoison = 25f;

    private List<ItemBehaviour> _itemsInCoffin;

    private Buff _storageCoffinBuff;

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
            item.OnItemActionPerformed -= ItemInCoffin_OnItemActionPerformed;
        }
    }

    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        _itemsInCoffin = _bag.ItemsInbag;

        foreach (var i in _itemsInCoffin)
        {
            i.OnItemActionPerformed += ItemInCoffin_OnItemActionPerformed;
        }
    }

    private void ItemInCoffin_OnItemActionPerformed(ItemBehaviour performedItem, Character targetCharacter)
    {
        bool isProc = UnityEngine.Random.Range(0f, 100f) <= _chanceToEnflictPoison ? true : false;
        
        if (isProc)
        {
            CombatManager.Instance.ApplyBuff(_storageCoffinBuff, targetCharacter);
        }
    }

    public void RemoveEffect()
    {

    }


}
