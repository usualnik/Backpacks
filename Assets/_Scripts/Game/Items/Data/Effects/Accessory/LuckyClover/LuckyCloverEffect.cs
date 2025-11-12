using UnityEngine;

public class LuckyCloverEffect : MonoBehaviour, IItemEffect
{
    private Buff _luckyCloverBuff;

    private void Awake()
    {
        _luckyCloverBuff = new Buff
        {
            Name = "LuckyCloverBuff",
            Type = Buff.BuffType.Luck,
            IsPositive = true,
            Value = 1
        };
    }
    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        CombatManager.Instance.ApplyBuff(_luckyCloverBuff, targetCharacter);
    }

    public void RemoveEffect()
    {
    }
}
