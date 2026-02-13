using UnityEngine;

public class GarlicStarEffect : MonoBehaviour, IStarEffect
{
    [Tooltip("Speed increase per unique STAR food type (e.g., 0.1 = 10% faster)")]
    [SerializeField] private float speedInreaseValue = 0.1f;
    private GarlicEffect _garlicEffect;

    private void Start()
    {
        _garlicEffect = GetComponent<GarlicEffect>();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _garlicEffect.CooldownMultiplier += speedInreaseValue;
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
       _garlicEffect.CooldownMultiplier -= speedInreaseValue;
    }
    
}