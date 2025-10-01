using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Items/Melee Weapon Data")]
public class MeleeWeaponDataSO : ItemDataSO
{
    private const ItemType ITEM_TYPE = ItemType.MeleeWeapons;
    public ItemType GetItemType() => ITEM_TYPE;
}
