using UnityEngine;

public class PocketSendEffect : MonoBehaviour, IItemEffect
{
    private Buff _pocketSendBuff;

    private void Awake()
    {
        _pocketSendBuff = new Buff
        {
            Name = "PocketSendBuff",
            Type = Buff.BuffType.Blindness,
            IsPositive = false,
            Value = 1
        };
    }

    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {
        CombatManager.Instance.ApplyBuff(_pocketSendBuff, targetCharacter);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

   
}
