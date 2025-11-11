using System.Collections;
using UnityEngine;

public class GarlicEffect : MonoBehaviour, IItemEffect
{
    private float _effectCooldown = 4f;

    private ItemBehaviour _itemBehaviour;
    private ItemEffectSO _effectDataSO;



    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += Combatmanager_OnCombatFinished;
    }

   
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= Combatmanager_OnCombatFinished;
    }

    private void Combatmanager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopCoroutine(GarlicArmorRoutine());
    }

    private void CombatManager_OnCombatStarted()
    {
        StartCoroutine(GarlicArmorRoutine());
    }

    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
        _itemBehaviour = target;
        _effectDataSO = effectData;

        CombatManager.Instance.ApplyEffect(_itemBehaviour, effectData);
    }

    public void RemoveEffect()
    {

    }
    private IEnumerator GarlicArmorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_effectCooldown);
            CombatManager.Instance.ApplyEffect(_itemBehaviour, _effectDataSO);
        }
    }


}
