using UnityEngine;

public class HealingHerbsEffect : MonoBehaviour,IItemEffect
{
    [SerializeField]
    private Buff _healingHerbsBuff;

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_healingHerbsBuff);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

}
