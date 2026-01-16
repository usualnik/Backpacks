using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _characterNickText;

    [Header("HEALTH")]
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private TextMeshProUGUI _characterHealthText;

    [Header("BLOCK")]
    [SerializeField] private TextMeshProUGUI _blockValueText;

    [Header("STAMINA")]
    [SerializeField] private TextMeshProUGUI _characterStaminaText;
    [SerializeField] private Image _staminaBarImage;

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
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

        InitPanel();
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        ClearBuffsText();
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;
        _character.OnCharacterStatsChanged -= Character_OnCharacterStatsChanged;
        _character.OnNewBuffApplied -= Character_OnNewEffectApplied;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }
    private void InitPanel()
    {

        switch (_panelType)
        {
            case PanelType.Player:
                _character = CombatManager.Instance.GetPlayerCharacter();
                _characterNickText.text = _character.NickName;
                _character.OnCharacterStatsChanged += Character_OnCharacterStatsChanged;
                _character.OnNewBuffApplied += Character_OnNewEffectApplied;
                break;
            case PanelType.Enemy:
                _character = CombatManager.Instance.GetEnemyCharacter();
                _characterNickText.text = _character.NickName;
                _character.OnCharacterStatsChanged += Character_OnCharacterStatsChanged;
                _character.OnNewBuffApplied += Character_OnNewEffectApplied;

                break;
            default:
                break;
        }
    }

    private void Character_OnCharacterDeath()
    {
        ClearBuffsText();
    }

    private void Character_OnNewEffectApplied(Buff buff)
    {
        UpdateBuffText(buff.Type.ToString(), buff.IsPositive);

    }

    private void Character_OnCharacterStatsChanged(Character.CharacterStats obj)
    {
        UpdateBars();
    }

    private void CombatManager_OnCombatStarted()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        _characterHealthText.text = "Health: " + _character.Stats.Health.ToString("F1");
        _healthBarImage.fillAmount = _character.Stats.Health / _character.Stats.HealthMax;

        _blockValueText.text = _character.Stats.Armor.ToString();

        _characterStaminaText.text = "Stamina: " + _character.Stats.Stamina.ToString("F1");
        _staminaBarImage.fillAmount = _character.Stats.Stamina / _character.Stats.StaminaMax;
    }

    private void UpdateBuffText(string buffName, bool isBuff)
    {
        if (isBuff)
        {
            _characterBuffsText.text += _characterBuffsText.text == string.Empty ? buffName + " " :
                "\n" + buffName + " ";

        }
        else
        {
            _characterDebuffsText.text += _characterDebuffsText.text == string.Empty ? buffName :
                "\n" + buffName;

        }
    }

    private void ClearBuffsText()
    {
        _characterBuffsText.text = string.Empty;
        _characterDebuffsText.text = string.Empty;
    }
}
