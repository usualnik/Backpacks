using System;
using UnityEngine;

public class WaitForCooldown : CustomYieldInstruction
{
    private ICooldownable _cooldownable;
    private float _startTime;
    private float _cachedCooldown; 

    public WaitForCooldown(ICooldownable cooldownable)
    {
        _cooldownable = cooldownable;
        _startTime = Time.time;
        _cachedCooldown = cooldownable.CurrentCooldown;
    }

    public override bool keepWaiting
    {
        get
        {
            float elapsed = Time.time - _startTime;
            float currentCooldown = _cooldownable.CurrentCooldown;

            if (Math.Abs(_cachedCooldown - currentCooldown) > 0.001f)
            {
                float progress = elapsed / _cachedCooldown;
                _startTime = Time.time - (progress * currentCooldown);
                _cachedCooldown = currentCooldown;
            }

            return elapsed < currentCooldown;
        }
    }
}