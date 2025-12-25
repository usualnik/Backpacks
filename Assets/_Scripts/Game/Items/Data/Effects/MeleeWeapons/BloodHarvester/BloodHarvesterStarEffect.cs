using UnityEngine;

public class BloodHarvesterStarEffect : MonoBehaviour, IStarEffect
{
    [SerializeField] private float _vampirismIncreasePercent = 100f;
    [SerializeField] private float _attacksFasterForEveryVampirismStackPercent = 5f;

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {

    }
 
}
