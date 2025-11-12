using UnityEngine;

public class GarlicStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private bool _isEffectApplied = false;
    [SerializeField] private StarCell[] _starCells;

    private void Start()
    {
        _starCells = GetComponentsInChildren<StarCell>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
       
        bool hasFilledStar = false;
        foreach (var star in _starCells)
        {
            if (star.IsFilled)
            {
                hasFilledStar = true;
                break; 
            }
        }

        //Применить эффект, только если предметы разные
        if (hasFilledStar && !_isEffectApplied 
            && sourceItem.ItemData.ItemName != targetItem.ItemData.ItemName)
        {
            _isEffectApplied = true;
            Debug.Log("Star Effect applied");
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        bool hasFilledStar = false;
        foreach (var star in _starCells)
        {
            if (star.IsFilled)
            {
                hasFilledStar = true;
                break; 
            }
        }

        if (!hasFilledStar && _isEffectApplied)
        {
            _isEffectApplied = false;
            Debug.Log("Star Effect Removed");
        }
    }
    
}