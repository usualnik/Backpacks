using System.Collections.Generic;
using UnityEngine;

public class PoisonGoobertStarEffect : MonoBehaviour
{
    [SerializeField] private List<ItemBehaviour> _trackedItems;

    [SerializeField] private Buff _poisonGoobertBuff;
    [SerializeField] private float _healAmount = 9f;

    private const int ITEM_ACTIVATIONS_NEEDED_TO_PROC = 5;
    private int _itemsActivations;

    private Character _ownerCharacter;
    private Character _targetCharacter;


    private void Awake()
    {
        _trackedItems = new List<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;


    }
    private void CombatManager_OnCombatStarted()
    {
        TrackStaredItems();
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopTrackItems();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_trackedItems.Contains(targetItem))
        {
            _trackedItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_trackedItems.Contains(targetItem))
        {
            _trackedItems.Remove(targetItem);
        }
    }

    private void TrackStaredItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate += GoobertStarEffect_OnEffectAcivate;
        }
    }

    private void StopTrackItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate -= GoobertStarEffect_OnEffectAcivate;
        }

        _itemsActivations = 0;
    }

    private void GoobertStarEffect_OnEffectAcivate()
    {
        CountActivations();
    }

    private void CountActivations()
    {
        _itemsActivations++;

        if (_itemsActivations >= ITEM_ACTIVATIONS_NEEDED_TO_PROC)
        {
            GoobertEffect();
            _itemsActivations = 0;
        }
    }

    private void GoobertEffect()
    {
        _ownerCharacter = GetComponent<ItemBehaviour>().OwnerCharacter;
        _targetCharacter = GetComponent<ItemBehaviour>().TargetCharacter;


        if (_ownerCharacter != null && _targetCharacter != null)
        {
            _ownerCharacter.AddHealth(_healAmount);
            _targetCharacter.ApplyBuff(_poisonGoobertBuff);
        }
    }

   
}

