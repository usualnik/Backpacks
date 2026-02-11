using System.Collections.Generic;
using UnityEngine;

public class FluteStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _speedIncrease = 0.1f;

    private float _speedIncreaseResult = 0.0f;

    private List<ItemBehaviour> _itemsInStar = new List<ItemBehaviour>();

    private FluteEffect fluteEffect;

    private void Awake()
    {
        fluteEffect = GetComponent<FluteEffect>();
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


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Remove(targetItem);
        }
    }

    private void CombatManager_OnCombatStarted()
    {
        if (_itemsInStar.Count == 0) return;

        foreach (var item in _itemsInStar)
        {
            _speedIncreaseResult += _speedIncrease;
        }

        fluteEffect.IncreaseSpeed(_speedIncreaseResult);
    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        fluteEffect.IncreaseSpeed(-_speedIncreaseResult);
        _speedIncreaseResult = 0;

    }
}
