using System;
using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public event Action OnCombatStarted;
    public event Action<CombatResult> OnCombatFinished;

    public event Action<ItemDataSO, string> OnDamageDealt;
    public event Action<ItemEffectSO, string> OnEffectAppliedDealt;




    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private EnemyCharacter _enemyCharacter;

    public enum CombatResult
    {
        PlayerWin,
        EnemyWin,
        Tie
    }
    private CombatResult _result;


    //private string _combatResult = string.Empty;

    private bool _isInCombat = false;

    private Coroutine _currentAttackRoutine;

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

        if (_currentAttackRoutine != null)
            StopCoroutine(_currentAttackRoutine);
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
        if (_currentAttackRoutine != null)
            StopCoroutine(_currentAttackRoutine);
        OnCombatFinished?.Invoke(_result);
    }

    private void Player_OnCharacterDeath()
    {
        _result = CombatResult.EnemyWin;
        _isInCombat = false;
        if (_currentAttackRoutine != null)
            StopCoroutine(_currentAttackRoutine);
        OnCombatFinished?.Invoke(_result);
    }
    #endregion

    #region WeaponDamage
    public void StartAutoAttack(ItemBehaviour.Target target,ItemDataSO attackWeapon,
        float damageMin, float damageMax,
        float staminaCost, float cooldown, float accuracy)
    {
        if (!_isInCombat)
        {
            Debug.Log("Cannot attack - combat not active");
            return;
        }

        switch (target)
        {
            case ItemBehaviour.Target.Player:
                _currentAttackRoutine = StartCoroutine(AutoAttackRoutine(_playerCharacter, attackWeapon,
                    damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            case ItemBehaviour.Target.Enemy:
                _currentAttackRoutine = StartCoroutine(AutoAttackRoutine(_enemyCharacter,attackWeapon,
                    damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            default:
                Debug.LogWarning($"Unknown target: {target}");
                break;
        }
    }


    private IEnumerator AutoAttackRoutine(Character targetCharacter, ItemDataSO attackWeapon,
        float damageMin, float damageMax,
        float staminaCost, float cooldown, float accuracy)
    {

        while (_isInCombat && targetCharacter != null && !targetCharacter.IsDead)
        {
            float damage = UnityEngine.Random.Range(damageMin, damageMax);
            damage = Mathf.RoundToInt(damage);

            targetCharacter.UseStamina(staminaCost);
            targetCharacter.TakeDamage(damage);

            OnDamageDealt?.Invoke(attackWeapon, targetCharacter.name);

            yield return new WaitForSeconds(cooldown);
        }

        Debug.Log("Auto attack routine finished");
        _currentAttackRoutine = null;
    }

    public void StopAutoAttack()
    {
        if (_currentAttackRoutine != null)
        {
            StopCoroutine(_currentAttackRoutine);
            _currentAttackRoutine = null;
            Debug.Log("Auto attack stopped");
        }
    }

    public PlayerCharacter GetPlayerCharacter() => _playerCharacter;
    public EnemyCharacter GetEnemyCharacter() => _enemyCharacter;
    #endregion

    #region Effects
    public void ApplyEffect(ItemBehaviour.Target target,  ItemEffectSO itemEffectSO)
    {
        switch (target)
        {
            case ItemBehaviour.Target.Player:
                _playerCharacter.ApplyEffect(itemEffectSO);
                OnEffectAppliedDealt?.Invoke(itemEffectSO, target.ToString());
                break;
            case ItemBehaviour.Target.Enemy:
                _enemyCharacter.ApplyEffect(itemEffectSO);
                OnEffectAppliedDealt?.Invoke(itemEffectSO, target.ToString());
                break;
            default:
                Debug.LogWarning($"Unknown target: {target}");
                break;
        }
        #endregion
    }
}