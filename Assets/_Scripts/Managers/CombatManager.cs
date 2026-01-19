using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class AttackRoutineData
{
    public Coroutine Routine { get; set; }
    public WeaponBehaviour Weapon { get; set; }
    public Character SourceCharacter { get; set; }
    public Character TargetCharacter { get; set; }
    public OwnerTargetHandler.Target TargetType { get; set; }
    public bool IsPaused { get; set; }

    public AttackRoutineData(Coroutine routine, WeaponBehaviour weapon, Character source,
                           Character target, OwnerTargetHandler.Target targetType)
    {
        Routine = routine;
        Weapon = weapon;
        SourceCharacter = source;
        TargetCharacter = target;
        TargetType = targetType;
        IsPaused = false;
    }
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public event Action OnCombatStarted;
    public event Action<CombatResult> OnCombatFinished;
    public event Action<WeaponBehaviour, Character, float> OnDamageDealt;
    public event Action<Character,float> OnCharacterStuned;

    public bool IsInCombat => _isInCombat;

    public event Action<int> OnFatigueDamageApplied;
    public event Action OnFatigueDamageStarted;
    public event Action OnFatigueDamageFinished;

    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private EnemyCharacter _enemyCharacter;

    public enum CombatResult
    {
        PlayerWin,
        EnemyWin,
        Tie
    }

    private CombatResult _result;

    private bool _isInCombat = false;
    private List<AttackRoutineData> _activeAttackRoutines = new List<AttackRoutineData>();

    private int _fatigueDamageAmount = 0;

    private float _fatigueDamageTimer = 0;
    private bool _isFatugueDamageStarted = false;

    private const float FATIGUE_DAMAGE_START_TIMER = 17f;
    private const float FATIGUE_DAMAGE_STEP = 1f;

    private Coroutine _fatigueDamageCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of combat manager");
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += StartCombatButton_OnGameStateChanged;

        _playerCharacter.OnCharacterDeath += Player_OnCharacterDeath;
        _enemyCharacter.OnCharacterDeath += Enemy_OnCharacterDeath;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStateChanged -= StartCombatButton_OnGameStateChanged;

        _playerCharacter.OnCharacterDeath -= Player_OnCharacterDeath;
        _enemyCharacter.OnCharacterDeath -= Enemy_OnCharacterDeath;

        StopAllAutoAttacks();
        StopFatigueDamage();
    }

    #region Events

    private void StartCombatButton_OnGameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Gameplay)
        {
            _isInCombat = true;
            OnCombatStarted?.Invoke();
        }
    }

    private void Enemy_OnCharacterDeath()
    {
        _result = CombatResult.PlayerWin;
        _isInCombat = false;
        StopAllAutoAttacks();
        StopFatigueDamage();
        OnCombatFinished?.Invoke(_result);
    }

    private void Player_OnCharacterDeath()
    {
        _result = CombatResult.EnemyWin;
        _isInCombat = false;
        StopAllAutoAttacks();
        StopFatigueDamage();
        OnCombatFinished?.Invoke(_result);
    }
    #endregion

    #region WeaponDamage
    public void StartAutoAttack(OwnerTargetHandler.Target target, WeaponBehaviour weapon,
    float damageMin, float damageMax,
    float staminaCost, float cooldown, float accuracy)
    {
        if (!_isInCombat)
        {
            Debug.Log("Cannot attack - combat not active");
            return;
        }

        AttackRoutineData routineData = null;
        switch (target)
        {
            case OwnerTargetHandler.Target.Player:
                routineData = CreateAttackRoutine(_enemyCharacter, _playerCharacter, weapon, target);
                break;
            case OwnerTargetHandler.Target.Enemy:
                routineData = CreateAttackRoutine(_playerCharacter, _enemyCharacter, weapon, target);
                break;
            default:
                Debug.LogWarning($"Unknown target: {target}");
                break;
        }

        if (routineData != null)
        {
            _activeAttackRoutines.Add(routineData);
        }
    }
    private void Update()
    {
        if (_isInCombat)
        {
            CleanupFinishedRoutines();

            if (!_isFatugueDamageStarted)
            {
                _fatigueDamageTimer += Time.deltaTime;

                if (_fatigueDamageTimer >= FATIGUE_DAMAGE_START_TIMER)
                {
                    StartFatigueDamage();
                    _isFatugueDamageStarted = true;
                }
            }
           
        }
    }
    private AttackRoutineData CreateAttackRoutine(Character sourceCharacter,
    Character targetCharacter, WeaponBehaviour weapon,OwnerTargetHandler.Target targetType)
    {
        var routineData = new AttackRoutineData(null, weapon, sourceCharacter, targetCharacter, targetType);
        routineData.Routine = StartCoroutine(AutoAttackRoutine(routineData));

        _activeAttackRoutines.Add(routineData);

        return routineData;
    }

    private IEnumerator AutoAttackRoutine(AttackRoutineData routineData)
    {
        var sourceCharacter = routineData.SourceCharacter;
        var targetCharacter = routineData.TargetCharacter;
        var weapon = routineData.Weapon;

        try
        {
            while (_isInCombat && targetCharacter != null && !targetCharacter.IsDead && sourceCharacter != null && !sourceCharacter.IsDead)
            {
                if (routineData.IsPaused)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                float damageMin = weapon.WeaponDamageMin;
                float damageMax = weapon.WeaponDamageMax;
                float staminaCost = weapon.WeaponDataSO.StaminaCost;
                float cooldown = weapon.WeaponDataSO.Cooldown;
                float accuracy = weapon.WeaponDataSO.Accuracy;

                float damage = CalculateFinalDamage(sourceCharacter, targetCharacter, weapon);
                damage = Mathf.RoundToInt(damage);

                bool isHit = UnityEngine.Random.Range(0f, 100f) <=
                CalculateFinalAccuracy(sourceCharacter, accuracy);

                if (!isHit)
                {
                    yield return new WaitForSeconds(cooldown);
                    continue;
                }

                if (sourceCharacter.HasStaminaToAttack(staminaCost))
                {
                    sourceCharacter.UseStamina(staminaCost);
                    targetCharacter.TakeDamage(damage, weapon.WeaponDataSO.ItemExtraType);

                    DealThornsDamageToAttacker(targetCharacter, sourceCharacter, damage);

                    CalculateVampirismHealing(sourceCharacter, damage);

                    OnDamageDealt?.Invoke(weapon, targetCharacter, damage);
                }
                else
                {
                    weapon.TryGetComponent(out WeaponVisual weaponVisual);
                    weaponVisual.ShowNoStaminaText();
                }

                yield return new WaitForSeconds(cooldown);
            }
        }
        finally
        {
            RemoveFinishedRoutine(routineData);
        }

    }

    private void CalculateVampirismHealing(Character sourceCharacter, float damageToVictim)
    {
        float vampirismStacks = (float) sourceCharacter.GetBuffStacks(Buff.BuffType.Vampirism);

        if (vampirismStacks > 0 && vampirismStacks <= damageToVictim)
        {
            sourceCharacter.AddHealth((float) vampirismStacks * sourceCharacter.LifeStealMultiplier);
        }
        else if (vampirismStacks > 0 && vampirismStacks > damageToVictim)
        {
            sourceCharacter.AddHealth(damageToVictim * sourceCharacter.LifeStealMultiplier);

        }
    }

    private float CalculateFinalDamage(Character sourceCharacter, Character targetCharacter, WeaponBehaviour weapon)
    {
        bool isCrit = UnityEngine.Random.Range(1f, 100f) <= weapon.CritHitChance;

        bool isCritResist = UnityEngine.Random.Range(1f, 100f) <= targetCharacter.CritHitResistChance;

        bool isIgnoreArmor = UnityEngine.Random.Range(1f, 100f) <= sourceCharacter.IgnoreArmorChance; 

        float armorStacks = isIgnoreArmor ? 0f : targetCharacter.GetArmorValue();

        float baseDamage = UnityEngine.Random.Range(weapon.WeaponDamageMin, weapon.WeaponDamageMax);

        if (isCrit && !isCritResist)
        {
            baseDamage *= 2f; 
        }

        float finalDamage = Mathf.Max(baseDamage - armorStacks, weapon.WeaponDamageMin * 0.1f); // Минимум 10% от минимального урона

        float finalDamageWithEmpowerBuff = finalDamage + sourceCharacter.GetBuffStacks(Buff.BuffType.Empower);

        return finalDamageWithEmpowerBuff;
    }

    private float CalculateFinalAccuracy(Character attacker, float weaponAccuracy)
    {
        float finalAccuracy;
        return finalAccuracy = attacker.GetAccuracyValue() + weaponAccuracy;
    }

    private void DealThornsDamageToAttacker(Character victim, Character attacker, float damageToVictim)
    {
        float thornsStacks = victim.GetBuffStacks(Buff.BuffType.Thorns);

        if (thornsStacks <= damageToVictim)
        {
            attacker.TakeDamage(thornsStacks, ItemDataSO.ExtraType.Nature);
        }
        else if(thornsStacks > damageToVictim)
        {
            attacker.TakeDamage(damageToVictim, ItemDataSO.ExtraType.Nature);
        }
    }

    public void StunCharacter(Character characterToStun, float stunDuration)
    {

        bool isCharacterHasResist = characterToStun.StunResistChance > 0;
        bool isResistProc = false;

        if (isCharacterHasResist)
        {
            isResistProc = UnityEngine.Random.Range(1f, 100f) <= characterToStun.StunResistChance ? true : false ;
        }

        if (characterToStun == null || characterToStun.IsDead || isResistProc)
        {
            return;
        }

        var routinesToPause = _activeAttackRoutines
            .Where(data => data.SourceCharacter == characterToStun)
            .ToList();

        if (routinesToPause.Count == 0)
        {
                      
            return;
        }

        OnCharacterStuned?.Invoke(characterToStun, stunDuration);

        StartCoroutine(StunRoutine(routinesToPause, stunDuration));
    }

    private IEnumerator StunRoutine(List<AttackRoutineData> routinesToPause, float stunDuration)
    {
        foreach (var routineData in routinesToPause)
        {
            routineData.IsPaused = true;
        }

        yield return new WaitForSeconds(stunDuration);

        foreach (var routineData in routinesToPause)
        {
            routineData.IsPaused = false;
        }
    }

    public void StopSpecificAutoAttack(WeaponBehaviour weapon)
    {
        var routinesToStop = _activeAttackRoutines.Where(data => data.Weapon == weapon).ToList();

        foreach (var routineData in routinesToStop)
        {
            if (routineData.Routine != null)
                StopCoroutine(routineData.Routine);
            _activeAttackRoutines.Remove(routineData);
        }
    }

    public void StopCharacterAttacks(Character character)
    {
        var routinesToStop = _activeAttackRoutines
            .Where(data => data.SourceCharacter == character || data.TargetCharacter == character)
            .ToList();

        foreach (var routineData in routinesToStop)
        {
            if (routineData.Routine != null)
                StopCoroutine(routineData.Routine);
            _activeAttackRoutines.Remove(routineData);
        }
    }

    public List<AttackRoutineData> GetActiveAttacksByWeapon(WeaponBehaviour weapon)
    {
        return _activeAttackRoutines.Where(data => data.Weapon == weapon).ToList();
    }

    public List<AttackRoutineData> GetActiveAttacksByCharacter(Character character)
    {
        return _activeAttackRoutines.Where(data =>
            data.SourceCharacter == character || data.TargetCharacter == character).ToList();
    }

    public List<AttackRoutineData> GetAllActiveAttacks()
    {
        return new List<AttackRoutineData>(_activeAttackRoutines);
    }

    public void StopAllAutoAttacks()
    {
        foreach (var routineData in _activeAttackRoutines)
        {
            if (routineData.Routine != null)
                StopCoroutine(routineData.Routine);
        }
        _activeAttackRoutines.Clear();
    }

    private void RemoveFinishedRoutine(AttackRoutineData routineData)
    {
        _activeAttackRoutines.Remove(routineData);
    }

    private void CleanupFinishedRoutines()
    {
        int removedCount = _activeAttackRoutines.RemoveAll(routineData =>
            routineData.SourceCharacter == null ||
            routineData.SourceCharacter.IsDead ||
            routineData.TargetCharacter == null ||
            routineData.TargetCharacter.IsDead ||
            routineData.Routine == null);

      
    }
    public void AttackCharacterOnce(Character sourceCharacter,Character targetCharacter, WeaponBehaviour weapon)
    {


        float damage = CalculateFinalDamage(sourceCharacter, targetCharacter, weapon);

        bool isHit = UnityEngine.Random.Range(0f, 100f) <=
                CalculateFinalAccuracy(sourceCharacter, weapon.WeaponDataSO.Accuracy);

        if (!isHit)
        {
            return;
        }


        if (sourceCharacter.HasStaminaToAttack(weapon.WeaponDataSO.StaminaCost))
        {
            sourceCharacter.UseStamina(weapon.WeaponDataSO.StaminaCost);
            targetCharacter.TakeDamage(damage, weapon.WeaponDataSO.ItemExtraType);

            DealThornsDamageToAttacker(targetCharacter, sourceCharacter, damage);
            OnDamageDealt?.Invoke(weapon, targetCharacter, damage);
        }
        else
        {
            weapon.TryGetComponent(out WeaponVisual weaponVisual);
            weaponVisual.ShowNoStaminaText();
        }

    }
   
    #endregion

    #region Fatigue Damage Management

    private void StartFatigueDamage()
    {       

        if (_fatigueDamageCoroutine == null)
        {
            ResetFatigueDamage();

            _fatigueDamageCoroutine = StartCoroutine(FatigueDamageRoutine());

            OnFatigueDamageStarted?.Invoke();

        }
    }

    private void StopFatigueDamage()
    {
        if (_fatigueDamageCoroutine != null)
        {
            StopCoroutine(_fatigueDamageCoroutine);
            _fatigueDamageCoroutine = null;
        }

        _fatigueDamageTimer = 0f;
        _isFatugueDamageStarted = false;

        ResetFatigueDamage();

        OnFatigueDamageFinished?.Invoke();

    }

    private void ResetFatigueDamage()
    {
        _fatigueDamageAmount = 0;
    }

    private IEnumerator FatigueDamageRoutine()
    {
        while (_isInCombat)
        {
            _fatigueDamageAmount++;

            _playerCharacter.TakeDamage(_fatigueDamageAmount,ItemDataSO.ExtraType.None);
            _enemyCharacter.TakeDamage(_fatigueDamageAmount, ItemDataSO.ExtraType.None);

            OnFatigueDamageApplied?.Invoke(_fatigueDamageAmount);

            yield return new WaitForSeconds(FATIGUE_DAMAGE_STEP);
        }
    }

    #endregion

    public PlayerCharacter GetPlayerCharacter() => _playerCharacter;
    public EnemyCharacter GetEnemyCharacter() => _enemyCharacter;

}