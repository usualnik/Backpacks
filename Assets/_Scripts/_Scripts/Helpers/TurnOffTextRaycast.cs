using TMPro;
using UnityEngine;

public class TurnOffTextRaycast : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        _textMeshPro.raycastTarget = false;
    }
}
