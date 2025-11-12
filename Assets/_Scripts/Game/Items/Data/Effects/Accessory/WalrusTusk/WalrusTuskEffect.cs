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
    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {      
        CombatManager.Instance.ApplyBuff(_walrusTuskBuff, targetCharacter);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
