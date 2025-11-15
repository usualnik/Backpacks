using UnityEngine;

public class PumpkinStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float _speedInrease = 0.1f;

    private WeaponBehaviour _pumpkin;

    private void Awake()
    {
        _pumpkin = GetComponent<WeaponBehaviour>();
    }
    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _pumpkin.IncreaseSpeed(_speedInrease);
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _pumpkin.IncreaseSpeed(-_speedInrease);

    }
}
