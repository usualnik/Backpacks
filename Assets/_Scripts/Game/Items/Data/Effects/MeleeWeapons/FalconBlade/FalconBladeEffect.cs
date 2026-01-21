using System;
using UnityEngine;

public class FalconBladeEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;


    private WeaponBehaviour _falconBlade;

    private void Awake()
    {
        _falconBlade = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;

    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_falconBlade == weapon)
        {
            AttackTwice();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    private void AttackTwice()
    {
        if (CombatManager.Instance.IsInCombat)
        {
            CombatManager.Instance.AttackCharacterOnce(_falconBlade.OwnerCharacter, _falconBlade.TargetCharacter, _falconBlade);
            OnActivate();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }


}
