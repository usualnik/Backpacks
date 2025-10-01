using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDataSO", menuName = "Items/Shield Data")]
public class ShieldDataSO : ItemDataSO
{
    private const ItemType ITEM_TYPE = ItemType.Shields;
    public ItemType GetItemType() => ITEM_TYPE;
}
