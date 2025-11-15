using UnityEngine;

public class PocketSendEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private Buff _pocketSendBuff;    

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_pocketSendBuff);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

   
}
