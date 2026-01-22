using System;
using UnityEngine;

public class ThornWhipEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; } = 0;
    public event Action OnEffectAcivate;

    [SerializeField] private Buff _thornsBuff;
    [SerializeField] private float _damageBuff = 1f;

    private WeaponBehaviour _thornWhip;

    private void Awake()
    {
        _thornWhip = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_thornWhip.OwnerCharacter)
        {
            _thornWhip.OwnerCharacter.OnNewBuffApplied -= OwnerCharacter_OnNewBuffApplied;
            _thornWhip.OwnerCharacter.OnBuffRemoved -= OwnerCharacter_OnBuffRemoved;
        }
    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_thornWhip == weapon)
        {
            GainThornsOnHit();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_thornWhip.OwnerCharacter)
        {
            _thornWhip.OwnerCharacter.OnNewBuffApplied += OwnerCharacter_OnNewBuffApplied;
            _thornWhip.OwnerCharacter.OnBuffRemoved += OwnerCharacter_OnBuffRemoved;
        }
    }

    private void OwnerCharacter_OnBuffRemoved(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Thorns)
        {
            DecreaseDamage();
        }
    }

    private void OwnerCharacter_OnNewBuffApplied(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Thorns)
        {
            GainDamage();
        }
    }

    public void RemoveEffect()
    {
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();

    }

    private void GainThornsOnHit()
    {
        if (_thornWhip.OwnerCharacter)
        {
            _thornWhip.OwnerCharacter.ApplyBuff(_thornsBuff);
        }
    }

    private void GainDamage()
    {
        if (_thornWhip)
        {
            _thornWhip.AddDamageToWeapon(_damageBuff);
        }
    }

    private void DecreaseDamage()
    {
        if (_thornWhip)
        {
            _thornWhip.AddDamageToWeapon(-_damageBuff);
        }
    }
}
