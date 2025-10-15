using UnityEngine;

[CreateAssetMenu(fileName = "AccessoryDataSO", menuName = "Items/Accessory Data")]
public class AccessoryDataSO : ItemDataSO
{
    public ItemType Type => ITEM_TYPE;

    private const ItemType ITEM_TYPE = ItemType.Accessory;

    public override void PerformAction(ItemBehaviour.Target target)
    {
       
    }
}
