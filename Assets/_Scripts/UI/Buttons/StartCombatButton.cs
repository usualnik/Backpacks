using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartCombatButton : MonoBehaviour
{
    public static event Action OnStartCombatButtonPressed;

    private Button _startCombatButton;
    private TextMeshProUGUI _startCombatButtonText;

    private const float LOOKING_FOR_OPPONENT_DELAY = 2.0f;

    private const string START_BATTLE_TEXT = "Start backpack battle";
    private const string LOOKING_FOR_OPPONENT_TEXT = "Looking for opponent...";
    private const int GAMEPLAY_WINDOW_INDEX = 2;


    private void Awake()
    {
        _startCombatButton = GetComponent<Button>();
        _startCombatButtonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _startCombatButton.onClick.AddListener(Click);
    }
    private void OnDestroy()
    {
        _startCombatButton.onClick.RemoveListener(Click);

    }
    private void Click()
    {
        OnStartCombatButtonPressed?.Invoke();

        _startCombatButtonText.text = LOOKING_FOR_OPPONENT_TEXT;
        Invoke(nameof(SimulateOpponentFound),LOOKING_FOR_OPPONENT_DELAY);
    }

    private void SimulateOpponentFound()
    {
        _startCombatButtonText.text = START_BATTLE_TEXT;
        WindowManager.Instance.OpenWindow(GAMEPLAY_WINDOW_INDEX);
    }
}
