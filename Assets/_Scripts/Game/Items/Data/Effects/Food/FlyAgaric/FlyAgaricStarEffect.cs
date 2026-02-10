using UnityEngine;

public class FlyAgaricStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float speedInreaseValue = 0.1f;
    private IFoodEffect _flyAgaricEffect;

    private void Start()
    {
        _flyAgaricEffect = GetComponent<IFoodEffect>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _flyAgaricEffect.IncreaseFoodSpeed(speedInreaseValue);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _flyAgaricEffect.IncreaseFoodSpeed(-speedInreaseValue);
    }
}
