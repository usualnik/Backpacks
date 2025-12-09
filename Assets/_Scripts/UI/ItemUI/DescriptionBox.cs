using TMPro;
using UnityEngine;

public class DescriptionBox : MonoBehaviour
{
    private ItemBehaviour _itemBehaviour;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;
    [SerializeField] private TextMeshProUGUI _itemSubTypeText;
    [SerializeField] private TextMeshProUGUI _itemRarityText;

    private void Awake()
    {
        _itemBehaviour = GetComponentInParent<ItemBehaviour>();
    }
    private void OnEnable()
    {

        _itemNameText.text = "Item Name : " + _itemBehaviour.ItemData.ItemName;
        _itemPriceText.text = "Item Price : " + _itemBehaviour.GetItemPrice().ToString();
        _itemSubTypeText.text = "Item SubType : " + _itemBehaviour.ItemData.ItemExtraType.ToString();
        _itemRarityText.text = "Item Rarity : " + _itemBehaviour.ItemData.Rarity.ToString();
    }
}
