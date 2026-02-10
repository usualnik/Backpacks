using UnityEngine;

public class CarrotStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float speedInreaseValue = 0.1f;

    private IFoodEffect _carrotEffect;

    private void Start()
    {
        _carrotEffect = GetComponent<IFoodEffect>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _carrotEffect.IncreaseFoodSpeed(speedInreaseValue);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _carrotEffect.IncreaseFoodSpeed(-speedInreaseValue);
    }
}
