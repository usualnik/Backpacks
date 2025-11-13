using UnityEngine;

public class HealingHerbsEffect : MonoBehaviour,IItemEffect
{
    private Buff _healingHerbsBuff;

    private void Awake()
    {
        _healingHerbsBuff = new Buff
        {
            Name = "HealingHerbsEffect",
            Type = Buff.BuffType.Regeneration,
            IsPositive = true,
            Value = 2
        };
    }
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_healingHerbsBuff);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

}
