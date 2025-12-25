using System.Collections;
using UnityEngine;

public class BananaEffect : MonoBehaviour, IItemEffect
{
    [SerializeField]
    private float _healAmount = 4f;

    [SerializeField]
    private float _bananaEffectCooldown = 5f;
    [SerializeField]
    private float _regenStaminaAmount = 1f;

    private Character _targetCharacter;

    private float _currentCooldownMultiplier = 1f;

    private Coroutine _bananaRoutine;

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_bananaRoutine != null)
        {
            StopCoroutine(BananaRoutine());
            _bananaRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_bananaRoutine != null)
        {
            StopCoroutine(BananaRoutine());
            _bananaRoutine = null;
        }
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _targetCharacter = targetCharacter;

        if (_bananaRoutine == null)
        {
            _bananaRoutine = StartCoroutine(BananaRoutine());
        }
    }

    public void RemoveEffect()
    {

    }

    private IEnumerator BananaRoutine()
    {
        while (true)
        {
            _targetCharacter.ChangeHealthValue(_healAmount);
            _targetCharacter.AddStamina(_regenStaminaAmount);

            float currentCooldown = _bananaEffectCooldown / _currentCooldownMultiplier;
            yield return new WaitForSeconds(currentCooldown);            
        }
    }

    public void IncreaseSpeed(float percentageIncrease)
    {
        _currentCooldownMultiplier += percentageIncrease;

        if (_bananaRoutine != null)
        {
            StopCoroutine(_bananaRoutine);
            _bananaRoutine = StartCoroutine(BananaRoutine());
        }
    }
}
