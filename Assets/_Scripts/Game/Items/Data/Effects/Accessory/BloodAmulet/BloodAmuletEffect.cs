using UnityEngine;

public class BloodAmuletEffect : MonoBehaviour, IItemEffect
{
    private float _additionalHealth = 20f;
    private Buff _bloodAmuletBuff;

    private void Awake()
    {
        _bloodAmuletBuff = new Buff
        {
            Name = "BloodAmuletBuff",
            Type = Buff.BuffType.Vampirism,
            IsPositive = true,
            Value = 2
        };
    }

    public void ApplyEffect(ItemBehaviour target, Character targetCharacter)
    {
        targetCharacter.ChangeHealthValue(_additionalHealth);

        CombatManager.Instance.ApplyBuff(_bloodAmuletBuff, targetCharacter);

    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
