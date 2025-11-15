using UnityEngine;

public class BananaStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float speedInrease = 0.1f;
    private BananaEffect _bananaEffect;

    private void Awake()
    {
        _bananaEffect = GetComponent<BananaEffect>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _bananaEffect.IncreaseSpeed(speedInrease);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _bananaEffect.IncreaseSpeed(-speedInrease);

    }

}
