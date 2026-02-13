using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Character : MonoBehaviour, IDamageable, IStaminable
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
    public event Action<Buff> OnNewBuffApplied;
    public event Action<Buff> OnBuffRemoved;
    public event Action OnCharacterDeath;
    public event Action OnStaminaEmpty;
    public event Action<float> OnDamageRecived;
    public event Action<float> OnHealingRecived;

    [SerializeField] protected string _nickname = string.Empty;
    [SerializeField] protected ClassDataSO _classData;
    [SerializeField] protected int _currentClassIndex = 0;

    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected CharacterStats _stats = new CharacterStats();
    [SerializeField] private List<Buff> _buffs;
    [SerializeField] private List<Buff> _debuffs;

    [Header("System refs")]
    [SerializeField] private BaseInventory _characterInventory;

    protected Character character;

    /*first element is default gold*/
    private int[] _levelGoldData = { 0, 12, 9, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15 };
    private const int SEVENTH_LEVEL_GOLD_BONUS = 10;
    private const int GOLD_BONUS_ROUND = 7;

    /*first element is class default health*/
    private int[] _levelHealthData = { 0, 25, 35, 45, 55, 70, 85, 100, 115, 130, 150, 170, 190, 210, 230, 260, 290, 320 };


    //----------------HEALTH---------------------

    private const float HEALTH_REGEN_STEP = 1f;
    private float _healthRegenMultiplier = 1f;
    private Coroutine _healthRegenCoroutine;

    //----------------STAMINA---------------------

    private const float STAMINA_REGEN_STEP = 0.1f;
    private float _staminaRegenMultiplier = 1f;
    private Coroutine _staminaRegenCoroutine;


    //----------------ACCURACY---------------------

    private const int ACCURACY_PER_STACK = 5;

    //----------------DAMAGE---------------------

    private CharacterDamageHandler _damageHandler;
    private float _ignoreArmorChance;

    private float _stunResistChance = 0f;
    private float _criticalHitResistChance = 0f;


    //-----------------BUFFS-----------------------

    private CharacterBuffHandler _buffHandler;

    //----------------Poison---------------------

    private float _poisonResistChance = 0f;

    private const float POISON_DAMAGE_STEP = 1f;
    private float _poisonMultiplier = 1f;
    private Coroutine _poisonCoroutine;

    //----------------Vampirism/Lifesteal---------

    private float _lifestealMultiplier = 1f;

    //---------------Helpers----------------------

    //This is just 100%
    private const float MAX_CHANCE = 100;

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
        _damageHandler = GetComponent<CharacterDamageHandler>();
        _buffHandler = GetComponent<CharacterBuffHandler>();

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
        ResetCharacterConfigAfterCombat();
    }

    protected void InvokeStatsChanged(CharacterStats stats)
    {
        OnCharacterStatsChanged?.Invoke(stats);
    }

    #endregion

    #region GameLogic

    // ---------------HELPERS----------------------------
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
    private void ResetCharacterConfigAfterCombat()
    {
        _buffs.Clear();
        _debuffs.Clear();

        _stats.Armor = 0f;
    }

    //----------------BUFFS------------------------------------
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

        OnNewBuffApplied?.Invoke(buff);
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
                    OnBuffRemoved?.Invoke(updatedBuff);
                }
                else
                {
                    _buffs[i] = updatedBuff;
                    OnBuffRemoved?.Invoke(updatedBuff);
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
                    OnBuffRemoved?.Invoke(updatedBuff);

                }
                else
                {
                    _debuffs[i] = updatedBuff;
                    OnBuffRemoved?.Invoke(updatedBuff);
                }
            }
        }

    }


    //-----------------DAMAGE-----------------------------------------
    public void TakeDamage(float damage, ItemDataSO.ExtraType weaponType)
    {
        float finalDamage = damage;


        switch (weaponType)
        {
            default:
                //Любой источник урона, например Fatigue или Thorns стаки, effect damage и пр.
                break;
            case ItemDataSO.ExtraType.Melee:
                if (_damageHandler != null)
                {
                    finalDamage = _damageHandler.FilterMeleeDamage(damage);
                }
                break;

            case ItemDataSO.ExtraType.Ranged:
                if (_damageHandler != null)
                {
                    finalDamage = _damageHandler.FilterRangedDamage(damage);
                }
                break;
        }

        if (_stats.Health - finalDamage > 0)
        {
            _stats.Health -= finalDamage;
            OnDamageRecived?.Invoke(finalDamage);

            InvokeStatsChanged(_stats);
        }
        else
        {
            OnCharacterDeath?.Invoke();
        }
    }

    //-----------------STAMINA------------------------------------------------
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
            _stats.Stamina += STAMINA_REGEN_STEP * _staminaRegenMultiplier;
            InvokeStatsChanged(_stats);
            yield return new WaitForSeconds(0.1f);
        }
        _staminaRegenCoroutine = null;
    }

    public void AddStamina(float value)
    {
        if (value > 0)
        {
            _stats.Stamina = Mathf.Min(_stats.Stamina + value, _stats.StaminaMax);
            InvokeStatsChanged(_stats);
        }
    }
    public void AddStaminaRegenStepMultiplier(float value)
    {
        _staminaRegenMultiplier += value;
    }

    public void ChangeStaminaMaxValue(float value)
    {
        _stats.StaminaMax += value;
    }

    //---------------------HEALTH----------------------------------

    public void ChangeMaxHealthValue(float value)
    {
        _stats.HealthMax += value;
    }

    public void AddHealth(float value)
    {
        _stats.Health = Mathf.Min(_stats.Health + value, _stats.HealthMax);

        InvokeStatsChanged(_stats);

        OnHealingRecived?.Invoke(value);
    }

    public void AddHealthRegenMultiplier(float value)
    {
        _healthRegenMultiplier += value;
    }

    public void HealthRegen()
    {
        if (_healthRegenCoroutine == null)
        {
            _healthRegenCoroutine = StartCoroutine(RegenHealthRoutine());
        }
    }

    private IEnumerator RegenHealthRoutine()
    {
        while (!_isDead && character.GetBuffStacks(Buff.BuffType.Regeneration) > 0)
        {
            float healingAmount = HEALTH_REGEN_STEP *
                character.GetBuffStacks(Buff.BuffType.Regeneration)
                * _healthRegenMultiplier;

            AddHealth(healingAmount);

            OnHealingRecived?.Invoke(healingAmount);

            yield return new WaitForSeconds(2f);
        }

        _healthRegenCoroutine = null;
    }

    //----------------POISON------------------------

    public void PoisonCharacter()
    {
        if (_poisonCoroutine == null)
        {
            _poisonCoroutine = StartCoroutine(PoisonRoutine());
        }
    }

    private IEnumerator PoisonRoutine()
    {

        while (!_isDead && character.GetDebuffStacks(Buff.BuffType.Poison) > 0)
        {
            TakeDamage(POISON_DAMAGE_STEP * _poisonMultiplier * character.GetDebuffStacks(Buff.BuffType.Poison), ItemDataSO.ExtraType.Effect);

            yield return new WaitForSeconds(2f);
        }

        _poisonCoroutine = null;
    }


    //----------------ARMOR---------------------

    public void AddArmor(float value)
    {
        if (_stats.Armor + value >= 0)
        {
            _stats.Armor += value;
        }

        InvokeStatsChanged(_stats);
    }

    public void AddIgnoreArmorChance(float chance)
    {
        _ignoreArmorChance += chance;
    }


    //----------------RESISTS---------------------
    public void AddPoisonResistChance(float chance)
    {
        if (_poisonResistChance + chance > MAX_CHANCE)
        {
            _poisonResistChance += chance;
        }
        else
        {
            _poisonResistChance += chance;
        }
    }

    public void AddStunResistChance(float chance)
    {
        if (_stunResistChance + chance > MAX_CHANCE)
        {
            _stunResistChance = MAX_CHANCE;
        }
        else
        {
            _stunResistChance += chance;
        }
    }
    public void AddCriticalHitResistChance(float chance)
    {
        if (_criticalHitResistChance + chance > MAX_CHANCE)
        {
            _criticalHitResistChance = MAX_CHANCE;
        }
        else
        {
            _criticalHitResistChance += chance;
        }
    }

    //----------------Vampirism/Lifesteal----------
    public void AddLifestealMultiplier(float value)
    {
        _lifestealMultiplier += value;
    }


    //----------------Reflect/Resist Buffs------------------------
    public void AddReflectStacks(int value)
    {
        _buffHandler.AddReflectStacks(value);
    }

    public void AddDebuffResistStacks(int value)
    {
        _buffHandler.AddDebuffResistStacks(value);
    }

    #endregion

    #region Getters

    public int GetBuffStacks(Buff.BuffType buffType)
    {
        int stacks = 0;

        foreach (var buff in _buffs)
        {
            if (buff.Type == buffType)
            {
                stacks++;
            }
        }

        return stacks;
    }

    public int GetDebuffStacks(Buff.BuffType buffType)
    {
        int stacks = 0;

        foreach (var buff in _debuffs)
        {
            if (buff.Type == buffType)
            {
                stacks++;
            }
        }

        return stacks;
    }

    public float GetAccuracyValue()
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

    public float GetArmorValue()
    {
        return _stats.Armor;
    }

    public int GetReflectStacks()
    {
         return _buffHandler.GetReflectStacks();    
    }

    public int GetDebuffResistStacks()
    {
        return _buffHandler.GetDebuffResistStacks();
    }

    public string NickName => _nickname;
    public CharacterStats Stats => _stats;
    public bool IsDead => _isDead;
    public float LifeStealMultiplier => _lifestealMultiplier;
    public float IgnoreArmorChance => _ignoreArmorChance;
    public float StunResistChance => _stunResistChance;
    public float CritHitResistChance => _criticalHitResistChance;
    public float PoisonResistChance => _poisonResistChance;
    public ClassDataSO ClassData => _classData;

    public List<Buff> AllBuffs => _buffs;
    public List<Buff> AllDebuffs => _debuffs;

    public BaseInventory CharacterInventory => _characterInventory;


    #endregion

    //TODO: Добавить логгинг эффектов
    //------------------------ LOGGER LOGIC -------------

    //private void LogEffect(string effectName, string targetName)
    //{
    //    string combatTimeText = "[" + TimeControlPanel.Instance.GetTimePassed().ToString("F1") + "]";

    //    GameObject newEffectLog = Instantiate(_logMessage, _content.transform.position, Quaternion.identity);
    //    newEffectLog.transform.SetParent(_content.transform, false);

    //    TextMeshProUGUI _effectText = newEffectLog.GetComponent<TextMeshProUGUI>();

    //    _effectText.text = combatTimeText + " AffectApplied " + "(" + effectName + ")";

    //    _effectText.color = targetName == "Player" ? _playerColor : _enemyColor;

    //}

}