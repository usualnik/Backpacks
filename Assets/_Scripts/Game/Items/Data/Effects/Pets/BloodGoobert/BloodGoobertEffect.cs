using System;
using UnityEngine;

public class BloodGoobertEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _bloodGoobertBuff;


    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        sourceCharacter.ApplyBuff(_bloodGoobertBuff);
        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
