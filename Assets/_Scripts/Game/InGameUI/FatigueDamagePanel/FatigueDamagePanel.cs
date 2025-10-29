using TMPro;
using UnityEngine;

public class FatigueDamagePanel : MonoBehaviour
{
    [SerializeField] private GameObject _fatigueDamageImageObject;
    [SerializeField] private TextMeshProUGUI _fatigueDamageText;

    private void Start()
    {
        CombatManager.Instance.OnFatigueDamageApplied += CombatManager_OnFatigueDamageApplied;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnFatigueDamageApplied -= CombatManager_OnFatigueDamageApplied;
    }

    private void CombatManager_OnFatigueDamageApplied(int fatigueDamageAmount)
    {
        if (!_fatigueDamageImageObject.activeInHierarchy)
            _fatigueDamageImageObject.SetActive(true);
        _fatigueDamageText.text = "Fatigue Damage Taken = " + fatigueDamageAmount;
    }
}
