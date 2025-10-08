using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private TextMeshProUGUI _winResultText;

    private const float HIDE_WIN_PANEL_TIMER = 3.0f;
    private void Awake()
    {
        _winPanel.SetActive(false);
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(string combatResult)
    {
        _winPanel.SetActive(true);
        _winResultText.text = combatResult;

        Invoke(nameof(HideWinResultPanel), HIDE_WIN_PANEL_TIMER);

    }

    private void HideWinResultPanel()
    {
        _winResultText.text = string.Empty;
        _winPanel.SetActive(false);
    }
}
