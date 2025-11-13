using System.Collections.Generic;
using UnityEngine;

public class CharacterDamageHandler : MonoBehaviour
{
    private List<IDamagePreventionEffect> _meleeDamagePreventionEffects = new List<IDamagePreventionEffect>();

    public void RegisterMeleeDamagePreventionEffect(IDamagePreventionEffect effect)
    {
        if (!_meleeDamagePreventionEffects.Contains(effect))
        {
            _meleeDamagePreventionEffects.Add(effect);
        }
    }

    public void UnRegisterMeleeDamagePreventionEffect(IDamagePreventionEffect effect)
    {
        _meleeDamagePreventionEffects.Remove(effect);
    }

    public float FilterMeleeDamage(float originalDamage)
    {
        float remainingDamage = originalDamage;

        foreach (var preventionEffect in _meleeDamagePreventionEffects)
        {
            if (preventionEffect.ShouldPreventDamage())
            {
                float preventedAmount = preventionEffect.GetPreventedDamageAmount();
                float actualPrevented = Mathf.Min(preventedAmount, remainingDamage);

                remainingDamage -= actualPrevented;

                if (remainingDamage <= 0)
                {
                    return 0f;
                }
            }
        }

        return Mathf.Max(0f, remainingDamage);
    }
}