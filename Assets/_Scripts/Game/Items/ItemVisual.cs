using UnityEngine;
using UnityEngine.UI;

public class ItemVisual : MonoBehaviour
{
    private Image _itemImage;

    private void Awake()
    {
        _itemImage = GetComponent<Image>();
    }

    public Image GetItemImage() => _itemImage;

    public void UpdateVisual(Sprite newVisual)
    {
        _itemImage.sprite = newVisual;
    }
}
