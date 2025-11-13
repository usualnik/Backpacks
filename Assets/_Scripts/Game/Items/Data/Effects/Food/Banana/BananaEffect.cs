using System.Collections;
using UnityEngine;

public class BananaEffect : MonoBehaviour, IItemEffect
{
    private float _healAmount = 4f;
    private float _bananaEffectCooldown = 5f;
    private float _regenStaminaAmount = 1f;

    private Character _targetCharacter;

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopCoroutine(BananaRoutine());
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        _targetCharacter = targetCharacter;

        StartCoroutine(BananaRoutine());
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

            yield return new WaitForSeconds(_bananaEffectCooldown);            
        }
    }
}
