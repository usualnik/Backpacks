using System;
using UnityEngine;

public class DeathScytheEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _critHitChanceBuff = 50f;

    private WeaponBehaviour _deathScythe;

    private int _poisonCounter = 0;
    private const int POISON_NEEDED_TO_PROC = 35;

    private void Awake()
    {
        _deathScythe = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _deathScythe.TargetCharacter.OnNewBuffApplied -= TargetCharacter_OnNewBuffApplied;
        _poisonCounter = 0;
    }

    public void OnActivate()
    {

    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_deathScythe.TargetCharacter)
        {
            _deathScythe.TargetCharacter.OnNewBuffApplied += TargetCharacter_OnNewBuffApplied;
        }
    }

    private void TargetCharacter_OnNewBuffApplied(Buff obj)
    {
        if (obj.Type == Buff.BuffType.Poison)
        {
            _poisonCounter++;

            if (_poisonCounter == POISON_NEEDED_TO_PROC)
            {
                ProcEffect();
                _poisonCounter = 0;
            }
        }
    }

    private void ProcEffect()
    {
        _deathScythe.AddCritHitChanceToWeapon(_critHitChanceBuff);
    }
}
