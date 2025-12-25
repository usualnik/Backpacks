using System.Collections;
using TMPro;
using UnityEngine;

public class BlueberriesEffect : MonoBehaviour
{
    [SerializeField] private float _baseEffectCooldown = 3.5f;
    [SerializeField] private Buff _manaBuff;
    [SerializeField] private Buff _accuracyBuff;
    [SerializeField] private int _switchBuffsAmount = 10;

    private Character _owner;

    private float _currentCooldownMultiplier = 1f;
    private Coroutine _blueberriesRoutine;

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += Combatmanager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= Combatmanager_OnCombatFinished;

        if (_blueberriesRoutine != null)
            StopCoroutine(_blueberriesRoutine);
    }

    private void Combatmanager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_blueberriesRoutine != null)
        {
            StopCoroutine(_blueberriesRoutine);
            _blueberriesRoutine = null;
        }
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (item == null || targetCharacter == null)
            return;

        _owner = targetCharacter;

        ApplyBlueberriesBuff();


        if (_blueberriesRoutine != null)
            StopCoroutine(_blueberriesRoutine);

        _blueberriesRoutine = StartCoroutine(BlueberriesRoutine());
    }

    public void RemoveEffect()
    {
        if (_blueberriesRoutine != null)
        {
            StopCoroutine(_blueberriesRoutine);
            _blueberriesRoutine = null;
        }
    }

    private IEnumerator BlueberriesRoutine()
    {
        while (true)
        {
            float currentCooldown = _baseEffectCooldown / _currentCooldownMultiplier;
            yield return new WaitForSeconds(currentCooldown);

          ApplyBlueberriesBuff();
        }
    }


    private void ApplyBlueberriesBuff() 
    {
        if (_owner.GetBuffStacks(Buff.BuffType.Mana) > _switchBuffsAmount)
        {
            _owner.ApplyBuff(_accuracyBuff);
        }
        else
        {
            _owner.ApplyBuff(_manaBuff);
        }
    }


    public void IncreaseSpeed(float percentageIncrease)
    {
        _currentCooldownMultiplier += percentageIncrease;

        if (_blueberriesRoutine != null)
        {
            StopCoroutine(_blueberriesRoutine);
            _blueberriesRoutine = StartCoroutine(BlueberriesRoutine());
        }
    }
}
