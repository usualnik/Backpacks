using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WinResultPanel : MonoBehaviour
{
    public static event Action OnWinResultPanelClicked;

    [Header("Panel refs")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _winResultText;
    [SerializeField] private TextMeshProUGUI _trophiesText;
    [SerializeField] private TextMeshProUGUI _livesText;

    [Space(10)]
    [SerializeField] private Button _showHideButton;
    

    private ClickOnWinPanel _clickOnWinPanel;

    private void Awake()
    {
        _panel.SetActive(false);
        _showHideButton.gameObject.SetActive(false);
        _clickOnWinPanel = _panel.GetComponent<ClickOnWinPanel>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
        _showHideButton.onClick.AddListener(OnShowHideButtonClick);
        _clickOnWinPanel.OnClick += ClickOnWinPanel_OnClick;
        PlayerCharacter.Instance.OnTrophiesValueChanged += UpdateTrophiesText;
        PlayerCharacter.Instance.OnLivesValueChanged += UpdateLivesText;


        InitTrophiesAndLivesText();
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _showHideButton.onClick.RemoveListener(OnShowHideButtonClick);
        _clickOnWinPanel.OnClick -= ClickOnWinPanel_OnClick;
        PlayerCharacter.Instance.OnTrophiesValueChanged -= UpdateTrophiesText;
        PlayerCharacter.Instance.OnLivesValueChanged -= UpdateLivesText;


    }

    private void ClickOnWinPanel_OnClick()
    {        
        _panel.SetActive(false );
        _showHideButton.gameObject.SetActive(false);

        OnWinResultPanelClicked?.Invoke();
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        InitWinPanel(combatResult);
    }

    private void InitWinPanel(CombatManager.CombatResult combatResult)
    {
        _panel.SetActive(true);
        _showHideButton.gameObject.SetActive(true);

        _winResultText.text = combatResult.ToString();      
    }

    private void UpdateTrophiesText(int value)
    {
        _trophiesText.text = "Trophies: " + value.ToString();
    }
    private void UpdateLivesText(int value)
    {
        _livesText.text = "Lives: " + value.ToString();
    }


    private void OnShowHideButtonClick()
    {
        if (_panel == null)
        {
            Debug.LogWarning("Win panel is null");
            return;
        }

        _panel.gameObject.SetActive(!_panel.gameObject.activeInHierarchy);
    }

    private void InitTrophiesAndLivesText()
    {
        _trophiesText.text = "Trophies: " + PlayerCharacter.Instance.Trophies.ToString();
        _livesText.text = "Lives: " + PlayerCharacter.Instance.Lives.ToString();
    }

}
