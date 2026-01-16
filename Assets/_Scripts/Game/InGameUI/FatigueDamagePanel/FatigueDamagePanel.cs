using TMPro;
using UnityEngine;

public class FatigueDamagePanel : MonoBehaviour
{
    [SerializeField] private GameObject _fatigueDamagePanelObject;
    [SerializeField] private TextMeshProUGUI _fatigueDamageText;

    private void Start()
    {
        CombatManager.Instance.OnFatigueDamageApplied += CombatManager_OnFatigueDamageApplied;
        CombatManager.Instance.OnFatigueDamageFinished += CombatManager_OnCombatFinished;
    }
   

    private void OnDestroy()
    {
        CombatManager.Instance.OnFatigueDamageApplied -= CombatManager_OnFatigueDamageApplied;
        CombatManager.Instance.OnFatigueDamageFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished()
    {
        HideFatigueDamagePanel();
    }


    private void CombatManager_OnFatigueDamageApplied(int fatigueDamageAmount)
    {
       UpdateFatigueDamagePanel(fatigueDamageAmount);
    }

    private void HideFatigueDamagePanel()
    {
        _fatigueDamagePanelObject.SetActive(false);
    }

    private void UpdateFatigueDamagePanel(int fatigueDamageAmount)
    {

        if (!_fatigueDamagePanelObject.activeInHierarchy && CombatManager.Instance.IsInCombat)
            _fatigueDamagePanelObject.SetActive(true);

        _fatigueDamageText.text = "Fatigue Damage Taken = " + fatigueDamageAmount;

    }
}
