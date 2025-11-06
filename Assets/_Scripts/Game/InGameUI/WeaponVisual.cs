using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private GameObject _noStaminaText;

    private const float HIDE_NO_STAMINA_TEXT_TIMER = 0.5f;
    public void ShowNoStaminaText()
    {
        _noStaminaText?.SetActive(true);
        Invoke(nameof(HideNoStaminaText), HIDE_NO_STAMINA_TEXT_TIMER);
    }
    private void HideNoStaminaText()
    {
        _noStaminaText?.SetActive(false);
    }
}
