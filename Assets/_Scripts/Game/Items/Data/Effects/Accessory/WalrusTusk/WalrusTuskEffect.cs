using UnityEngine;

public class WalrusTuskEffect : MonoBehaviour, IItemEffect
{
    private Buff _walrusTuskBuff;

    private void Awake()
    {

        _walrusTuskBuff = new Buff
        {
            Name = "WalrusTuskBuff",
            Type = Buff.BuffType.Thorns,
            IsPositive = true,
            Value = 1
        };

    }
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {      
       targetCharacter.ApplyBuff(_walrusTuskBuff);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
