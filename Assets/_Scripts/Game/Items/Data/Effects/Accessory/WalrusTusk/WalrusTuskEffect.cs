using UnityEngine;

public class WalrusTuskEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private Buff _walrusTuskBuff;

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {      
       targetCharacter.ApplyBuff(_walrusTuskBuff);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
