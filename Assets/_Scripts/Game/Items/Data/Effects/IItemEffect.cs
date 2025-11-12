using UnityEngine;

public interface IItemEffect
{
    public void ApplyEffect(ItemBehaviour item, Character targetCharacter);
    public void RemoveEffect();
}
