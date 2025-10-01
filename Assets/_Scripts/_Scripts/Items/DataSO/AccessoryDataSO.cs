using UnityEngine;

[CreateAssetMenu(fileName = "AccessoryDataSO", menuName = "Items/Accessory Data")]
public class AccessoryDataSO : ItemDataSO
{
    private const ItemType ITEM_TYPE = ItemType.Accessory;
    public ItemType GetItemType() => ITEM_TYPE;
}
