using System;
using UnityEngine;

public class BloodAmuletEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField]
    private float _additionalHealth = 20f;

    [SerializeField]
    private Buff _bloodAmuletBuff; 

    public void StartOfCombatInit(ItemBehaviour target, Character sourceCharacter, Character targetCharacter)
    {
        targetCharacter.ChangeMaxHealthValue(_additionalHealth);
        targetCharacter.AddHealth(_additionalHealth);
        targetCharacter.ApplyBuff(_bloodAmuletBuff);
        OnActivate();
    }     

    public void RemoveEffect()
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
