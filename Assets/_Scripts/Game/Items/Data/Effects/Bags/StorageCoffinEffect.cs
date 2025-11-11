using System.Collections.Generic;
using UnityEngine;

public class StorageCoffinEffect : MonoBehaviour, IItemEffect
{
    private Bag _bag;

    private float _poisonEffectAmount = 1f;
    private float _chanceToEnflictPoison = 25f;

    private List<ItemBehaviour> _itemsInCoffin;
    private ItemEffectSO _effectDataSO;

    private ItemBehaviour _target;

    private void Awake()
    {
        _bag = GetComponent<Bag>();
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

    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
        _itemsInCoffin = _bag.ItemsInbag;
        _effectDataSO = effectData;
        _target = target;


        foreach (var item in _itemsInCoffin)
        {
            item.OnItemActionPerformed += ItemInCoffin_OnItemActionPerformed;
        }
    }

    private void ItemInCoffin_OnItemActionPerformed(ItemBehaviour performedItem)
    {
        bool isProc = UnityEngine.Random.Range(0f, 100f) <= _chanceToEnflictPoison ? true : false;
        
        if (isProc)
        {
            CombatManager.Instance.ApplyEffect(_target, _effectDataSO);
        }
    }

    public void RemoveEffect()
    {

    }


}
