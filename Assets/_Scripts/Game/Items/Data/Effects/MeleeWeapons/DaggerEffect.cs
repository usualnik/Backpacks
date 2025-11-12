using UnityEngine;

public class DaggerEffect : MonoBehaviour, IItemEffect
{
    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        Debug.Log("[DAGGER EFFECT]");
    }

    public void RemoveEffect()
    {
    }
}
