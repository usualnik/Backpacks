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
        _playerNickText.text = "Name: " + PlayerCharacter.Instance.NickName;
        _playeClassText.text = "Class: " + PlayerCharacter.Instance.ClassName;
        _goldAmountText.text = "Gold: " + PlayerCharacter.Instance.Stats.GoldAmount;
        _healthAmountText.text = "Health: " + PlayerCharacter.Instance.Stats.Health;
        _staminaAmountText.text = "Stamina: " + PlayerCharacter.Instance.Stats.Stamina;
        _rankText.text = "Rank: " + PlayerCharacter.Instance.Stats.RankName;
    }
}
