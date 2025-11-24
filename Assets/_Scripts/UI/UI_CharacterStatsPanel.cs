using TMPro;
using UnityEngine;

public class UI_CharacterStatsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerNickText;
    [SerializeField] private TextMeshProUGUI _playeClassText;
    [SerializeField] private TextMeshProUGUI _goldAmountText;
    [SerializeField] private TextMeshProUGUI _healthAmountText;
    [SerializeField] private TextMeshProUGUI _staminaAmountText;
    [SerializeField] private TextMeshProUGUI _rankText;

    private void Start()
    {

        InitText();

        PlayerCharacter.Instance.OnCharacterStatsChanged += PlayerCharacter_OnCharacterStatsChanged;
        PlayerCharacter.Instance.OnPlayerClassChanged += PlayerCharacter_OnPlayerClassChanged;

    }


    private void OnDestroy()
    {
        PlayerCharacter.Instance.OnCharacterStatsChanged -= PlayerCharacter_OnCharacterStatsChanged;
        PlayerCharacter.Instance.OnPlayerClassChanged -= PlayerCharacter_OnPlayerClassChanged;
    }

    private void PlayerCharacter_OnCharacterStatsChanged(Character.CharacterStats stats)
    {
        UpdateText(stats);
    }

    private void PlayerCharacter_OnPlayerClassChanged(ClassDataSO newPlayerClass)
    {
        UpdatePlayerClassText(newPlayerClass.ClassName);
    }
    private void InitText()
    {
        _playerNickText.text = "Name: " + PlayerCharacter.Instance.NickName;
        _playeClassText.text = "Class: " + PlayerCharacter.Instance.ClassData.ClassName;
        _goldAmountText.text = "Gold: " + PlayerCharacter.Instance.Stats.GoldAmount;
        _healthAmountText.text = "Health: " + PlayerCharacter.Instance.Stats.Health;
        _staminaAmountText.text = "Stamina: " + PlayerCharacter.Instance.Stats.Stamina;
        _rankText.text = "Rank: " + PlayerCharacter.Instance.Rank;
    }

    private void UpdateText(Character.CharacterStats stats)
    {   
        
        _goldAmountText.text = "Gold: " + stats.GoldAmount;
        _healthAmountText.text = "Health: " + stats.Health;
        //_staminaAmountText.text = "Stamina: " + stats.Stamina;        
    }

    private void UpdatePlayerClassText(string newClassName)
    {
        _playeClassText.text = "Class: " + newClassName;
    }
}
