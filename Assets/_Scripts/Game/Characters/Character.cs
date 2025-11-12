using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Character : MonoBehaviour, IDamageable, IStaminable
{
    [System.Serializable]
    public class CharacterStats
    {
        public int GoldAmount = 0;
        public float Health = 0f;
        public float HealthMax = 0f;
        public float Armor = 0f;
        public float Stamina = 0f;
        public float StaminaMax = 0f;
    }

    public event Action<CharacterStats> OnCharacterStatsChanged;
    public event Action<Buff.BuffType, bool> OnNewBuffApplied;
    public event Action OnCharacterDeath;
    public event Action OnStaminaEmpty;

    [SerializeField] protected string _nickname = string.Empty;
    [SerializeField] protected string _className = string.Empty;


    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected CharacterStats _stats = new CharacterStats();
    [SerializeField] private List<Buff> _buffs;
    [SerializeField] private List<Buff> _debuffs;

    private const float STAMINA_REGEN_STEP = 0.1f;
    private Coroutine _staminaRegenCoroutine;

    /*first element is default gold*/
    private int[] _levelGoldData = { 0, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15 };
    private const int SEVENTH_LEVEL_GOLD_BONUS = 10;
    private const int GOLD_BONUS_ROUND = 7;

    /*first element is class default health*/
    private int[] _levelHealthData = { 0, 35, 45, 55, 70, 85, 100, 115, 130, 150, 170, 190, 210, 230, 260, 290, 320, 350 };

    private const int ACCURACY_PER_STACK = 5;

    #region Init + Events

    private void Awake()
    {
        _buffs = new List<Buff>();
        _debuffs = new List<Buff>();
    }
    private void Start()
    {
        InitializeCharacter();
    }

    private void OnDestroy()
    {
        DestroyCharacter();
    }

    protected virtual void InitializeCharacter()
    {
        _stats.Health = _stats.HealthMax;
        _stats.Stamina = _stats.StaminaMax;

        LevelManager.Instance.OnLevelChanged += LevelManager_OnLevelChanged;
    }

    protected virtual void DestroyCharacter()
    {
        LevelManager.Instance.OnLevelChanged -= LevelManager_OnLevelChanged;
    }


    protected void LevelManager_OnLevelChanged(int levelIndex)
    {
        AddHealthAndGoldAfterCombat(levelIndex);
    }
    #endregion
    #region WeaponDamage
    public void TakeDamage(float damage)
    {
        if (_stats.Health - damage > 0)
        {
            _stats.Health -= damage;
            InvokeStatsChanged(_stats);
        }
        else
        {
            OnCharacterDeath?.Invoke();
        }
    }

    public void UseStamina(float amount)
    {
        if (_stats.Stamina - amount > 0)
        {
            _stats.Stamina -= amount;
            _staminaRegenCoroutine = StartCoroutine(RegenStaminaRoutine());
        }
        else
        {
            OnStaminaEmpty?.Invoke();
        }
    }

    public bool HasStaminaToAttack(float amount)
    {
        return _stats.Stamina - amount > 0;
    }

    private IEnumerator RegenStaminaRoutine()
    {
        while (!_isDead && _stats.Stamina < _stats.StaminaMax)
        {
            _stats.Stamina += STAMINA_REGEN_STEP;
            InvokeStatsChanged(_stats);
            yield return new WaitForSeconds(0.1f);
        }
        _staminaRegenCoroutine = null;
    }
    #endregion   
    #region Getters
    public float GetAccuracy()
    {
        float accuracy = 0;
        foreach (var buff in _buffs)
        {
            if (buff.Type == Buff.BuffType.Luck)
            {
                accuracy += ACCURACY_PER_STACK;
            }
        }

        foreach (var debuff in _debuffs)
        {
            if (debuff.Type == Buff.BuffType.Blindness)
            {
                accuracy -= ACCURACY_PER_STACK;
            }
        }

        return accuracy;
    }

    public float GetLuckStacks()
    {
        float luckStacks = 0;

        foreach (var buff in _buffs)
        {
            if (buff.Type == Buff.BuffType.Luck)
            {
                luckStacks++;
            }
        }

        return luckStacks;
    }

    public float GetArmorStacks()
    {     
        return _stats.Armor;
    }

    public float GetThornsStacks()
    {
        float thornsStacks = 0;
        foreach (var buff in _buffs)
        {
            if (buff.Type == Buff.BuffType.Thorns)
            {
                thornsStacks++;
            }
        }

        return thornsStacks;
    }
    public string NickName => _nickname;
    public string ClassName => _className;
    public CharacterStats Stats => _stats;
    public bool IsDead => _isDead;

    #endregion
    #region Setters
    public void ChangeHealthValue(float value)
    {
        _stats.HealthMax += value;
        _stats.Health += value;
        InvokeStatsChanged(_stats);       
    }

    public void ChangeArmorValue(float value)
    {
        _stats.Armor += value;
        InvokeStatsChanged(_stats);
    }
    #endregion
    #region GameLogic
    private void AddHealthAndGoldAfterCombat(int levelIndex)
    {
        _stats.HealthMax = _levelHealthData[levelIndex];


        if (levelIndex == GOLD_BONUS_ROUND)
        {
            _stats.GoldAmount += _levelGoldData[levelIndex] + SEVENTH_LEVEL_GOLD_BONUS;
        }
        else
        {
            _stats.GoldAmount += _levelGoldData[levelIndex];
        }

        _stats.Health = _stats.HealthMax;
        _stats.Stamina = _stats.StaminaMax;

        InvokeStatsChanged(_stats);
    }

    protected void InvokeStatsChanged(CharacterStats stats)
    {
        OnCharacterStatsChanged?.Invoke(stats);
    }

    public void ApplyBuff(Buff buff)
    {
        if (buff.IsPositive)
        {
            _buffs.Add(buff);
        }
        else
        {
            _debuffs.Add(buff);
        }

        OnNewBuffApplied?.Invoke(buff.Type, buff.IsPositive);
    }

    public void RemoveBuff(Buff.BuffType buffTypeToRemove, int removeAmount)
    {
        if (removeAmount <= 0) return;

        for (int i = _buffs.Count - 1; i >= 0; i--)
        {
            if (_buffs[i].Type == buffTypeToRemove)
            {
                Buff updatedBuff = _buffs[i];
                updatedBuff.Value -= removeAmount;

                if (updatedBuff.Value <= 0)
                {
                    _buffs.RemoveAt(i);
                }
                else
                {
                    _buffs[i] = updatedBuff; 
                }
            }
        }

        for (int i = _debuffs.Count - 1; i >= 0; i--)
        {
            if (_debuffs[i].Type == buffTypeToRemove)
            {
                Buff updatedBuff = _debuffs[i];
                updatedBuff.Value -= removeAmount;

                if (updatedBuff.Value <= 0)
                {
                    _debuffs.RemoveAt(i);
                }
                else
                {
                    _debuffs[i] = updatedBuff; 
                }
            }
        }
    }

    #endregion

}