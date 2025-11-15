using UnityEngine;

public class BloodAmuletEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private float _additionalHealth = 20f;

    [SerializeField]
    private Buff _bloodAmuletBuff;

    public void ApplyEffect(ItemBehaviour target, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ChangeMaxHealthValue(_additionalHealth);
        targetCharacter.ChangeHealthValue(_additionalHealth);

        targetCharacter.ApplyBuff(_bloodAmuletBuff);
    }

    public void RemoveEffect()
    {

    }
}
