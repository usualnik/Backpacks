using System;
using UnityEngine;

public class PocketSendEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField]
    private Buff _pocketSendBuff;  

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_pocketSendBuff);
        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

}
