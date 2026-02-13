using System;
using UnityEngine;

public interface IPotionEffect
{
    public event Action OnPotionConsumed; 
    public void TriggerEffect();

    public void TriggerPotionEffect();

}
