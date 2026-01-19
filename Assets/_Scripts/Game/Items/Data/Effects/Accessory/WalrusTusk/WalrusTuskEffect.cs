using System;
using UnityEngine;

public class WalrusTuskEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField]
    private Buff _walrusTuskBuff;



    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {      
       targetCharacter.ApplyBuff(_walrusTuskBuff);
        OnActivate();
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }    

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
