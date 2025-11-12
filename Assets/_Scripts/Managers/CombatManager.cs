using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public event Action OnCombatStarted;
    public event Action<CombatResult> OnCombatFinished;
    public event Action<Buff,string> OnBuffApplied;


    public event Action<ItemDataSO, string> OnDamageDealt;
    public event Action<int> OnFatigueDamageApplied;

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
    private List<Coroutine> _activeAttackRoutines = new List<Coroutine>();

    private int _fatigueDamageAmount = 0;
    private const float _fatigueDamageStartTimer = 17f;
    private const float _fatigueDamageRepeatTimer = 1f;
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
            StartFatigueDamage();
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
    public void StartAutoAttack(ItemBehaviour.Target target, WeaponBehaviour weapon,
        float damageMin, float damageMax,
        float staminaCost, float cooldown, float accuracy)
    {
        if (!_isInCombat)
        {
            Debug.Log("Cannot attack - combat not active");
            return;
        }

        Coroutine newRoutine = null;
        switch (target)
        {
            case ItemBehaviour.Target.Player:
                newRoutine = StartCoroutine(AutoAttackRoutine(_enemyCharacter,
                    _playerCharacter, weapon,
                    damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            case ItemBehaviour.Target.Enemy:
                newRoutine = StartCoroutine(AutoAttackRoutine(_playerCharacter,
                    _enemyCharacter, weapon,
                    damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            default:
                Debug.LogWarning($"Unknown target: {target}");
                break;
        }

        if (newRoutine != null)
        {
            _activeAttackRoutines.Add(newRoutine);
        }
    }

    private IEnumerator AutoAttackRoutine(Character sourceCharacter,
        Character targetCharacter, WeaponBehaviour weapon,
        float damageMin, float damageMax,
        float staminaCost, float cooldown, float accuracy)
    {
        while (_isInCombat && targetCharacter != null && !targetCharacter.IsDead)
        {

            float damage = CalculateFinalDamage(sourceCharacter,targetCharacter, damageMin, damageMax, weapon);
            damage = Mathf.RoundToInt(damage);

            bool isHit = UnityEngine.Random.Range(0f, 100f) <=
                CalculateFinalAccuracy(sourceCharacter, accuracy) ? true : false;

            if (!isHit)
                yield return new WaitForSeconds(cooldown);

            if (sourceCharacter.HasStaminaToAttack(staminaCost))
            {
                sourceCharacter.UseStamina(staminaCost);
                targetCharacter.TakeDamage(damage);
                OnDamageDealt?.Invoke(weapon.ItemData, targetCharacter.name);
            }
            else
            {
                weapon.TryGetComponent(out WeaponVisual weaponVisual);
                weaponVisual.ShowNoStaminaText();
            }

            yield return new WaitForSeconds(cooldown);
        }

        // јвтоматически очищаем завершенные корутины при остановке бо€
        CleanupFinishedRoutines();
    }
    private float CalculateFinalDamage(Character sourceCharacter,Character targetCharcter ,float damageMin, float damageMax,
        WeaponBehaviour weapon)
    {
        if (weapon.ItemData.Type.HasFlag(ItemDataSO.ItemType.MeleeWeapons))
        {
            bool isCrit = UnityEngine.Random.Range(0f, 100f) <= weapon.CritHitChance ? true : false;

            if (isCrit)
            {
                return UnityEngine.Random.Range
                    ((weapon.WeaponDamageMin + sourceCharacter.GetThornsStacks() - targetCharcter.GetArmorStacks()) * 2,
                    (weapon.WeaponDamageMax + sourceCharacter.GetThornsStacks() - targetCharcter.GetArmorStacks()) * 2);
            }
            else
            {
                return UnityEngine.Random.Range
                    (weapon.WeaponDamageMin + sourceCharacter.GetThornsStacks() - targetCharcter.GetArmorStacks(),
                    weapon.WeaponDamageMax + sourceCharacter.GetThornsStacks() - targetCharcter.GetArmorStacks());
            }
           
        }
        else
        {
            // TODO:  огда добавишь рендж пухи - нужно написать формулу урона
            return 0;
        }

    }
    private float CalculateFinalAccuracy(Character attacker, float weaponAccuracy)
    {
        float finalAccuracy;

        return finalAccuracy = attacker.GetAccuracy() + weaponAccuracy;
    }

    public void StopAutoAttack(ItemDataSO weaponToStop)
    {
        // ќстанавливает все атаки от конкретного оружи€
        // ¬ данной реализации останавливает все атаки, так как нет прив€зки к конкретному оружию
        // ƒл€ более точного управлени€ можно использовать Dictionary<ItemDataSO, Coroutine>
        StopAllAutoAttacks();
    }

    public void StopAllAutoAttacks()
    {
        foreach (var routine in _activeAttackRoutines)
        {
            if (routine != null)
                StopCoroutine(routine);
        }
        _activeAttackRoutines.Clear();
    }

    private void CleanupFinishedRoutines()
    {
        // ”дал€ем null ссылки (завершенные корутины)
        _activeAttackRoutines.RemoveAll(routine => routine == null);
    }

    public PlayerCharacter GetPlayerCharacter() => _playerCharacter;
    public EnemyCharacter GetEnemyCharacter() => _enemyCharacter;
    #endregion

    #region Buffs
    public void ApplyBuff(Buff buff, Character targetCharacter)
    {     
        targetCharacter.ApplyBuff(buff);
        OnBuffApplied?.Invoke(buff,targetCharacter.NickName);

    }
    #endregion

    #region Fatigue Damage Management

    private void StartFatigueDamage()
    {
        ResetFatigueDamage();
        _fatigueDamageCoroutine = StartCoroutine(FatigueDamageRoutine());
    }

    private void StopFatigueDamage()
    {
        if (_fatigueDamageCoroutine != null)
        {
            StopCoroutine(_fatigueDamageCoroutine);
            _fatigueDamageCoroutine = null;
        }
        ResetFatigueDamage();
    }

    private void ResetFatigueDamage()
    {
        _fatigueDamageAmount = 0;
    }

    private IEnumerator FatigueDamageRoutine()
    {
        yield return new WaitForSeconds(_fatigueDamageStartTimer);

        while (_isInCombat)
        {
            _fatigueDamageAmount++;
            _playerCharacter.TakeDamage(_fatigueDamageAmount);
            _enemyCharacter.TakeDamage(_fatigueDamageAmount);
            OnFatigueDamageApplied?.Invoke(_fatigueDamageAmount);

            yield return new WaitForSeconds(_fatigueDamageRepeatTimer);
        }
    }

    #endregion
}