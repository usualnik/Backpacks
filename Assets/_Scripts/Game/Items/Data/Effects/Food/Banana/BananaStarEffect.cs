using UnityEngine;

public class BananaStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float speedInreaseValue;
    private BananaEffect _bananaEffect;

    private void Awake()
    {
        _bananaEffect = GetComponent<BananaEffect>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _bananaEffect.IncreaseSpeed(speedInreaseValue);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _bananaEffect.IncreaseSpeed(-speedInreaseValue);

    }

}
