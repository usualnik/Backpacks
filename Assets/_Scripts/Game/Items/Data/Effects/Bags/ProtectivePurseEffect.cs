using UnityEngine;

public class ProtectivePurseEffect : MonoBehaviour, IItemEffect
{
    private float _buffArmorAmount = 15f;

    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        targetCharacter.ChangeArmorValue(_buffArmorAmount);
    }

    public void RemoveEffect()
    {
    }

    
}
