using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInventory : MonoBehaviour
{
    public List<ItemBehaviour> ItemsInInventory => _itemsInIventory;
    [SerializeField] protected List<ItemBehaviour> _itemsInIventory;
}
