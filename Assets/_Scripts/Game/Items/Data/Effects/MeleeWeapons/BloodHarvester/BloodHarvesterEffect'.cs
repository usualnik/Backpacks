using System;
using UnityEngine;

public class BloodHarvesterEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get ; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _increaseSpeedMultiplier = 0.05f;

    private Character _owner;
    private WeaponBehaviour _bloodHarvester;


    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatMnager_OnCombatFinished;
        _bloodHarvester = GetComponent<WeaponBehaviour>();
    }

    private void CombatMnager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_owner)
        {
            _owner.OnNewBuffApplied -= Owner_OnNewBuffApplied;
            _bloodHarvester.ResetWeaponStatsToDefault();
        }

    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _owner = sourceCharacter;

        if (_owner)
        {
            _owner.OnNewBuffApplied += Owner_OnNewBuffApplied;
        }

    }

    private void Owner_OnNewBuffApplied(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Vampirism)
        {
            _bloodHarvester.IncreaseSpeedMultiplier(_increaseSpeedMultiplier);
            OnActivate();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

   
}
