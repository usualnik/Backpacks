using UnityEngine;

public class LuckyCloverEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private Buff _luckyCloverBuff;
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_luckyCloverBuff);
    }

    public void RemoveEffect()
    {
    }
}
