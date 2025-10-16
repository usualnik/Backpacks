using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class TimeControlPanel : MonoBehaviour
{
    public static TimeControlPanel Instance {  get; private set; }

    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI _gameSpeedText;
    [SerializeField] private Slider _gameSpeedSlider;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _speedUpButton;
    [SerializeField] private TextMeshProUGUI _gameTimePassedText;

    private float _gameSpeed = 1.0f;
    private float _gameTimePassed = 0.0f;
    private bool _isCombatStarted; 

    private const float GAME_SPEED_MAX = 3.0f;
    private const float SPEED_STEP = 0.1f;
    private const float NORMAL_GAME_SPEED = 1.0f;
    private const float PAUSE_GAME_SPEED = 0.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of time control panel");
    }


    private void Start()
    {       
        Init();
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        _gameSpeedSlider.onValueChanged.RemoveListener(OnGameSpeedSliderChanged);
        _pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        _speedUpButton.onClick.RemoveListener(OnSpeedUpButtonClicked);

        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void Init()
    {
        _gameSpeedSlider.minValue = 0f;
        _gameSpeedSlider.maxValue = GAME_SPEED_MAX;
        _gameSpeedSlider.value = _gameSpeed;

        UpdateSpeedDisplay();

        _gameSpeedSlider.onValueChanged.AddListener(OnGameSpeedSliderChanged);

        if (_pauseButton != null)
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
        if (_speedUpButton != null)
            _speedUpButton.onClick.AddListener(OnSpeedUpButtonClicked);
    }
    

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _gameSpeed = NORMAL_GAME_SPEED;
        _gameSpeedSlider.SetValueWithoutNotify(_gameSpeed);
        ApplyGameSpeed();
        UpdateSpeedDisplay();

        _isCombatStarted = false;

    }

    private void CombatManager_OnCombatStarted()
    {
        _isCombatStarted = true;
    }


    private void OnGameSpeedSliderChanged(float value)
    {
        _gameSpeed = SnapToStep(value);
        _gameSpeedSlider.SetValueWithoutNotify(_gameSpeed);

        UpdateSpeedDisplay();
        ApplyGameSpeed();
    }

    private float SnapToStep(float value)
    {       
        return Mathf.Round(value / SPEED_STEP) * SPEED_STEP;
    }

    private void UpdateSpeedDisplay()
    {
        _gameSpeedText.text = _gameSpeed.ToString("F1") + "x";
    }

    private void ApplyGameSpeed()
    {
        Time.timeScale = _gameSpeed;
    }

    private void OnPauseButtonClicked()
    {
        _gameSpeed = _gameSpeed <= PAUSE_GAME_SPEED ? NORMAL_GAME_SPEED : PAUSE_GAME_SPEED;

        _gameSpeedSlider.SetValueWithoutNotify(_gameSpeed);
        ApplyGameSpeed();
        UpdateSpeedDisplay();
    }
    private void OnSpeedUpButtonClicked()
    {
        if (_gameSpeed < GAME_SPEED_MAX)
        {           
            _gameSpeed = Mathf.RoundToInt(_gameSpeed += 1f);
            _gameSpeedSlider.SetValueWithoutNotify(_gameSpeed);
            ApplyGameSpeed();
            UpdateSpeedDisplay();
        }
       
    }
    private void FixedUpdate()
    {
        if (!_isCombatStarted)
            return;

        _gameTimePassed += Time.deltaTime;
        _gameTimePassedText.text = _gameTimePassed.ToString("F1");        
    }

    public float GetTimePassed() => _gameTimePassed;
}
