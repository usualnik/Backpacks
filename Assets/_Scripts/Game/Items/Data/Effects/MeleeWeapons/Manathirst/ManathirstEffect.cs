using System;
using UnityEngine;

public class ManathirstEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _manaBuff;
    [SerializeField] private float _damageOnProc = 10f;

    private WeaponBehaviour _manaThirst;

    private const int MANA_BUFFS_NEEDED_TO_PROC = 30;
    private int _manaBuffCounter = 0;

    private void Awake()
    {
        _manaThirst = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnDamageDealt;
        CombatManager.Instance.OnCombatFinished += CombatManager_CombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_CombatFinished;
        CombatManager.Instance.OnHit -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_CombatFinished(CombatManager.CombatResult obj)
    {
        if (_manaThirst.OwnerCharacter)
        {
            _manaThirst.OwnerCharacter.OnNewBuffApplied -= OwnerCharacter_OnNewBuffApplied;
        }
        _manaBuffCounter = 0;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_manaThirst == weapon)
        {
            OnHitManaBuff();
            OnActivate();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_manaThirst.OwnerCharacter)
        {
            _manaThirst.OwnerCharacter.OnNewBuffApplied += OwnerCharacter_OnNewBuffApplied;
        }
    }

    private void OwnerCharacter_OnNewBuffApplied(Buff newBuff)
    {
        if (newBuff.Type == Buff.BuffType.Mana)
        {
            _manaBuffCounter++;
            if (_manaBuffCounter == MANA_BUFFS_NEEDED_TO_PROC)
            {
                ProcDamage();
                _manaBuffCounter = 0;
            }
        }
    }

    private void OnHitManaBuff()
    {
        if (_manaThirst.OwnerCharacter == null) return;
        _manaThirst.OwnerCharacter.ApplyBuff(_manaBuff);
    }

    private void ProcDamage()
    {
        float damage = _damageOnProc + _manaThirst.OwnerCharacter.GetBuffStacks(Buff.BuffType.Vampirism);
        _manaThirst.TargetCharacter.TakeDamage(damage,ItemDataSO.ExtraType.Effect);
        _manaThirst.OwnerCharacter.AddHealth(damage);
    }
}
