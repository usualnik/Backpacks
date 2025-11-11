using UnityEngine;

public interface IItemEffect
{
    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData);
    public void RemoveEffect();
}
