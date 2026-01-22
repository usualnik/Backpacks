using System;
using UnityEngine;

public class FancyFencingRapierEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get ; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _luckBuff;
    [SerializeField] private float _damageBuff = 3f;
    private WeaponBehaviour _fancyRapier;

    private const int LUCK_STACKS_NEEDED_TO_PROC = 3;

    private void Awake()
    {
        _fancyRapier = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnMiss += CombatManager_OnMiss;
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }    

    private void OnDestroy()
    {
        CombatManager.Instance.OnMiss -= CombatManager_OnMiss;
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
    }



    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_fancyRapier == weapon && _fancyRapier.OwnerCharacter.GetBuffStacks(Buff.BuffType.Luck) >= LUCK_STACKS_NEEDED_TO_PROC)
        {
            RapierEffect();
            OnActivate();
        }
    }
    private void CombatManager_OnMiss(WeaponBehaviour weapon, Character arg2)
    {
        if (_fancyRapier == weapon)
        {
            ProcLuckBuff();
            OnActivate();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }
    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    private void ProcLuckBuff()
    {
        _fancyRapier.OwnerCharacter.ApplyBuff(_luckBuff);
    }

    private void RapierEffect()
    {
        _fancyRapier.OwnerCharacter.RemoveBuff(Buff.BuffType.Luck, LUCK_STACKS_NEEDED_TO_PROC);
        _fancyRapier.AddDamageToWeapon(_damageBuff);
    }
}
