using UnityEngine;

public class DaggerEffect : MonoBehaviour, IItemEffect
{
    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
        Debug.Log("[DAGGER EFFECT]");
    }

    public void RemoveEffect()
    {
    }
}
