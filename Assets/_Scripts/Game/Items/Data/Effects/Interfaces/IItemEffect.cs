
using System;

public interface IItemEffect
{    
    public int ItemActivations { get; set; }
    public event Action OnEffectAcivate;

    void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter);
    void OnActivate();
}