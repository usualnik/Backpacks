using UnityEngine;

public class ProtectivePurseEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private float _buffArmorAmount = 15f;

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ChangeArmorValue(_buffArmorAmount);
    }

    public void RemoveEffect()
    {
    }

    
}
