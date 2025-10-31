using TMPro;
using UnityEngine;

public class UI_RerollPriceText : MonoBehaviour
{
    private Shop _shop;
    private TextMeshProUGUI _rerollText;

    private void Awake()
    {
        _rerollText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _shop = GetComponentInParent<Shop>();

        _rerollText.text = _shop.GetCurrentRerollPrice().ToString();

        _shop.OnRerollPriceChanged += Shop_OnRerollPriceChanged;
    }
    private void OnDestroy()
    {
        _shop.OnRerollPriceChanged -= Shop_OnRerollPriceChanged;
    }

    private void Shop_OnRerollPriceChanged()
    {
        _rerollText.text = _shop.GetCurrentRerollPrice().ToString();
    }
}
