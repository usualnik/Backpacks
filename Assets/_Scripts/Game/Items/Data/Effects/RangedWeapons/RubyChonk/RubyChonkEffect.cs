using System;
using UnityEngine;

public class RubyChonkEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _heatBuff;
    [SerializeField] private int _heatAmountNeededToProc = 12;
    [SerializeField] private int _chanceToProc = 30;
    [SerializeField] private float _stunDuration = 0.4f;

    private WeaponBehaviour _rubyChonk;

    private void Awake()
    {
        _rubyChonk = GetComponent<WeaponBehaviour>();
    }

    void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {

        if (_rubyChonk == weapon)
        {
            GainHeatBuff();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
    }

    private void GainHeatBuff()
    {
        _rubyChonk.OwnerCharacter.ApplyBuff(_heatBuff);
        OnActivate();

        if (_rubyChonk.OwnerCharacter.GetBuffStacks(Buff.BuffType.Heat) > _heatAmountNeededToProc)
        {
            ProcTryStunOpponent();
        }
    }
    private void ProcTryStunOpponent()
    {
        bool isProcStun = UnityEngine.Random.Range(1, 101) < _chanceToProc;

        if (!isProcStun) return;

        if(!_rubyChonk.TargetCharacter) return;

        CombatManager.Instance.StunCharacter(_rubyChonk.TargetCharacter, _stunDuration);

    }


}
