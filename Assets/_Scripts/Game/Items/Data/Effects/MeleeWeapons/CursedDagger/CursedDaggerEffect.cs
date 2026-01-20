using System;
using UnityEngine;

public class CursedDaggerEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; } = 0;
    public event Action OnEffectAcivate;

    private WeaponBehaviour _daggerBehaviour;


    private void Awake()
    {
        _daggerBehaviour = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnCharacterStuned += CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;
    }

    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_daggerBehaviour == weapon)
        {
            InflictRandomDebuffs();
        }
    }
    private void CombatManager_OnCharacterStuned(Character stunnedCharacter, float stunDuration)
    {
        if (!_daggerBehaviour.OwnerCharacter)
        {
            return;
        }

        if (stunnedCharacter != null && stunnedCharacter == _daggerBehaviour.OwnerCharacter)
        {
            CombatManager.Instance.AttackCharacterOnce(_daggerBehaviour.OwnerCharacter, _daggerBehaviour.TargetCharacter, _daggerBehaviour);
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void RemoveEffect()
    {
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();

    }

    private void InflictRandomDebuffs()
    {
        Buff randomDebuff = GetRandomBuff();
        _daggerBehaviour.TargetCharacter.ApplyBuff(randomDebuff);

        Buff randomDebuff2= GetRandomBuff();
        _daggerBehaviour.TargetCharacter.ApplyBuff(randomDebuff2);

        OnActivate();

    }

    private Buff GetRandomBuff()
    {
        Buff.BuffType[] deBuffTypes = { Buff.BuffType.Poison, Buff.BuffType.Blindness, Buff.BuffType.Cold };

        int randomIndex = UnityEngine.Random.Range(0, deBuffTypes.Length);

        Buff constuctedBuff;

        switch (randomIndex)
        {
            case 0:
                constuctedBuff = new Buff
                {
                    Name = "CursedDaggerPoisonBuff",
                    IsPositive = false,
                    Type = Buff.BuffType.Poison,
                    Value = 1
                };
                break;
            case 1:
                constuctedBuff = new Buff
                {
                    Name = "CursedDaggerBlindBuff",
                    IsPositive = false,
                    Type = Buff.BuffType.Blindness,
                    Value = 1
                };
                break;
            case 2:
                constuctedBuff = new Buff
                {
                    Name = "CursedDaggerColdBuff",
                    IsPositive = false,
                    Type = Buff.BuffType.Cold,
                    Value = 1
                };
                break;


            //По дефоту отдаем баф яда
            default:
                constuctedBuff = new Buff
                {
                    Name = "CursedDaggerPoisonBuff",
                    IsPositive = false,
                    Type = Buff.BuffType.Poison,
                    Value = 1
                };
                break;
        }

        return constuctedBuff;

    }
}
