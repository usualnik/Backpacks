using System.Collections.Generic;
using UnityEngine;

public class PandamoniumStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private List<ItemBehaviour> _starredFood = new List<ItemBehaviour>();

    [SerializeField] private Buff _poisonBuff;

    private WeaponBehaviour _pandamonium;

    private void Awake()
    {
        _pandamonium = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_starredFood.Count > 0)
        {
            UnsubscribeToFoodActivates();
        }
    }

    private void CombatManager_OnCombatStarted()
    {
        if (_starredFood.Count > 0)
        {
            SubscribeToFoodActivates();
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_starredFood.Contains(targetItem))
        {
            _starredFood.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

        if (_starredFood.Contains(targetItem))
        {
            _starredFood.Remove(targetItem);
        }
    }
    private void SubscribeToFoodActivates()
    {
        foreach (var food in _starredFood)
        {
            food.TryGetComponent(out IItemEffect foodEffect);

            if (foodEffect != null)
            {
                foodEffect.OnEffectAcivate += FoodEffect_OnEffectAcivate;
            }
        }
    }

    private void UnsubscribeToFoodActivates()
    {
        foreach (var food in _starredFood)
        {
            food.TryGetComponent(out IItemEffect foodEffect);

            if (foodEffect != null)
            {
                foodEffect.OnEffectAcivate -= FoodEffect_OnEffectAcivate;
            }
        }
    }

    private void FoodEffect_OnEffectAcivate()
    {
        if (_pandamonium.TargetCharacter == null) return;

        _pandamonium.TargetCharacter.ApplyBuff(_poisonBuff);
    }
}
