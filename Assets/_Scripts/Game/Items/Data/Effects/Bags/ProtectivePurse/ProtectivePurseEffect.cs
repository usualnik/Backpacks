using System;
using UnityEngine;

public class ProtectivePurseEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    [SerializeField]
    private float _buffArmorAmount = 15f;

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.AddArmor(_buffArmorAmount);
        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

}
