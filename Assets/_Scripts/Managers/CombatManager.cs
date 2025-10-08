using System;
using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public event Action OnCombatStarted;
    public event Action<string> OnCombatFinished;

    [SerializeField] private PlayerCharacter _playerCharacter;
    [SerializeField] private EnemyCharacter _enemyCharacter;

    private string _combatResult = string.Empty;
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

    private void StartCombatButton_OnGameStateChanged(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Gameplay)
        {
            Debug.Log("I AM STARTING COMBAT");
            _isInCombat = true;
            OnCombatStarted?.Invoke();
        }
    }

    private void Enemy_OnCharacterDeath()
    {
        _combatResult = "PLAYER WINS";
        _isInCombat = false;
        if (_currentAttackRoutine != null)
            StopCoroutine(_currentAttackRoutine);
        OnCombatFinished?.Invoke(_combatResult);
    }

    private void Player_OnCharacterDeath()
    {
        _combatResult = "ENEMY WINS";
        _isInCombat = false;
        if (_currentAttackRoutine != null)
            StopCoroutine(_currentAttackRoutine);
        OnCombatFinished?.Invoke(_combatResult);
    }

    public void StartAutoAttack(ItemBehaviour.Target target,
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
                _currentAttackRoutine = StartCoroutine(AutoAttackRoutine(_playerCharacter, damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            case ItemBehaviour.Target.Enemy:
                _currentAttackRoutine = StartCoroutine(AutoAttackRoutine(_enemyCharacter, damageMin, damageMax, staminaCost, cooldown, accuracy));
                break;
            default:
                Debug.LogWarning($"Unknown target: {target}");
                break;
        }
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

    private IEnumerator AutoAttackRoutine(Character targetCharacter,
        float damageMin, float damageMax,
        float staminaCost, float cooldown, float accuracy)
    {
        Debug.Log($"Starting auto attack on {targetCharacter.name}");
       
        while (_isInCombat && targetCharacter != null && !targetCharacter.IsDead)
        {
           
            if (UnityEngine.Random.value > accuracy)
            {
                Debug.Log($"Attack missed! Accuracy: {accuracy}");
            }
            else
            {
                float damage = UnityEngine.Random.Range(damageMin, damageMax);
                Debug.Log($"Dealing {damage} damage to {targetCharacter.name}");

                targetCharacter.TakeDamage(damage);
            }

            yield return new WaitForSeconds(cooldown);
        }

        Debug.Log("Auto attack routine finished");
        _currentAttackRoutine = null;
    }


    public void ApplyEffect(ItemBehaviour.Target target)
    {
        // Реализация эффектов
    }
}