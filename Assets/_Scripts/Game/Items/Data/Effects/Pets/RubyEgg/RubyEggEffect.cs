using System;
using UnityEngine;

public class RubyEggEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _empowerBuff;
    [SerializeField] private int _reflectDebuffsAmount = 3;

    public void StartOfCombatInit(ItemBehaviour target, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ApplyBuff(_empowerBuff);
        targetCharacter.AddReflectStacks(_reflectDebuffsAmount);

        OnActivate();
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
