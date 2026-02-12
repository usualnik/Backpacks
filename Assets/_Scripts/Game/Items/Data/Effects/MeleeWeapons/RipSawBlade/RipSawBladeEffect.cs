using System;
using UnityEngine;

public class RipSawBladeEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private int _removeBuffFromOpponentAmount = 2;

    private WeaponBehaviour _ripSawBlade;

    private void Awake()
    {
        _ripSawBlade = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;

    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_ripSawBlade != weapon) return;
        if (_ripSawBlade.TargetCharacter == null) return;

        OnHitEffect();
    }

    private void OnHitEffect()
    {
        int removeBuffIndex = UnityEngine.Random.Range(0, 2);

        /*
         * 
         * 0 - remove regen buff
         * 
         * 1 - remove thorns buff
         * 
         */

        switch (removeBuffIndex)
        {
            case 0:
                _ripSawBlade.TargetCharacter.RemoveBuff(Buff.BuffType.Regeneration, _removeBuffFromOpponentAmount);
                break;

            case 1:
                _ripSawBlade.TargetCharacter.RemoveBuff(Buff.BuffType.Thorns, _removeBuffFromOpponentAmount);
                break;

            default:
                break;
        }

        OnActivate();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }


    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }



}
