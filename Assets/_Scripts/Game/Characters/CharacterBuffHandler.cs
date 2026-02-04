using UnityEngine;

public class CharacterBuffHandler : MonoBehaviour
{
    private Character _character;

    private int _reflectionStacks = 0;
    private bool _canReflect = true;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        _character.OnNewBuffApplied += Character_OnNewBuffApplied;
        _character.OnBuffRemoved += Character_OnBuffRemoved;
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        _character.OnNewBuffApplied -= Character_OnNewBuffApplied;
        _character.OnBuffRemoved -= Character_OnBuffRemoved;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _reflectionStacks = 0;
    }

    private void CombatManager_OnCombatStarted()
    {
        _canReflect = true;
    }

    private void Character_OnNewBuffApplied(Buff obj)
    {
        HandleNewBuffApplied(obj.Type);
    }

    private void Character_OnBuffRemoved(Buff obj)
    {
        HandleBuffRemoved(obj.Type);
    }

    private void HandleNewBuffApplied(Buff.BuffType buffType)
    {

        switch (buffType)
        {
            case Buff.BuffType.None:
                break;
            case Buff.BuffType.Empower:
                break;
            case Buff.BuffType.Heat:
                //Recalculate cooldowns
                break;
            case Buff.BuffType.Luck:
                break;
            case Buff.BuffType.Mana:
                break;
            case Buff.BuffType.Regeneration:
                _character.HealthRegen();
                break;
            case Buff.BuffType.Thorns:
                break;
            case Buff.BuffType.Vampirism:
                break;
            case Buff.BuffType.Poison:

                if (_canReflect && _reflectionStacks > 0)
                {
                    ReflectDebuff(buffType);
                }
                bool isProcPoisonResist = UnityEngine.Random.Range(1f, 100f) <= _character.PoisonResistChance;

                if (!isProcPoisonResist)
                    _character.PoisonCharacter();
                break;

            case Buff.BuffType.Blindness:

                if (_canReflect && _reflectionStacks > 0)
                {
                    ReflectDebuff(buffType);
                }

                break;

            case Buff.BuffType.Cold:

                if (_canReflect && _reflectionStacks > 0)
                {
                    ReflectDebuff(buffType);
                }

                //Recalculate cooldowns
                break;
            default:
                break;
        }
    }
   
    private void HandleBuffRemoved(Buff.BuffType buffType)
    {

        switch (buffType)
        {
            case Buff.BuffType.None:
                break;
            case Buff.BuffType.Empower:
                break;
            case Buff.BuffType.Heat:
                //Recalculate cooldowns
                break;
            case Buff.BuffType.Luck:
                break;
            case Buff.BuffType.Mana:
                break;
            case Buff.BuffType.Regeneration:
                break;
            case Buff.BuffType.Thorns:
                break;
            case Buff.BuffType.Vampirism:
                break;
            case Buff.BuffType.Poison:
                break;
            case Buff.BuffType.Blindness:
                break;
            case Buff.BuffType.Cold:
                //Recalculate cooldowns
                break;
            default:
                break;
        }
    }

    private void ReflectDebuff(Buff.BuffType buffTypeToReflect)
    {
        Character target;

        if (_character is PlayerCharacter)
            target = EnemyCharacter.Instance;
        else
            target = PlayerCharacter.Instance;


        for (int i = 0; i < _reflectionStacks; i++)
        {
            _character.RemoveBuff(buffTypeToReflect, 1);

            target.ApplyBuff(new Buff
            {
                Name = "ReflectedDebuff",
                Type = buffTypeToReflect,
                IsPositive = false,
                Value = 1,
            });

            Debug.Log(buffTypeToReflect.ToString() + "Reflected to " + target.NickName + " from " + _character.NickName);
        }

        Debug.Log("Total reflected amount = " + _reflectionStacks);

        _reflectionStacks = 0;

        _canReflect = false;
    }

    public void AddReflectStacks(int value)
    {
        _reflectionStacks += value;
    }

    public int GetReflectStacks() { return _reflectionStacks; }

}
