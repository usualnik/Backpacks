using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _characterNickText;
    [SerializeField] private Image _healthBarImage;

    [SerializeField] private TextMeshProUGUI _characterHealthText;
    [SerializeField] private TextMeshProUGUI _characterStaminaText;
    [SerializeField] private TextMeshProUGUI _characterBuffsText;
    [SerializeField] private TextMeshProUGUI _characterDebuffsText;

    private enum PanelType
    {
        Player,
        Enemy
    }

    [SerializeField] private PanelType _panelType = PanelType.Player;
    private Character _character;


    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        

        InitPanel();
    }

   
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        _character.OnCharacterStatsChanged -= Character_OnCharacterStatsChanged;

    }
    private void InitPanel()
    {

        switch (_panelType)
        {
            case PanelType.Player:
                _character = CombatManager.Instance.GetPlayerCharacter();
                _characterNickText.text = _character.NickName;
                _character.OnCharacterStatsChanged += Character_OnCharacterStatsChanged;
                break;
            case PanelType.Enemy:
                _character = CombatManager.Instance.GetEnemyCharacter();
                _characterNickText.text = _character.NickName;
                _character.OnCharacterStatsChanged += Character_OnCharacterStatsChanged;

                break;
            default:
                break;
        }
    }

    private void Character_OnCharacterStatsChanged(Character.CharacterStats obj)
    {
        UpdateText();
    }

    private void CombatManager_OnCombatStarted()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _characterHealthText.text = "Health: " + _character.Stats.Health.ToString("F2");
        _healthBarImage.fillAmount = _character.Stats.Health / _character.Stats.HealthMax;

        _characterStaminaText.text = "Stamina: " + _character.Stats.Stamina.ToString("F2");
        //bufs + debufs

    }
}
