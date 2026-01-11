using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    public List<ItemBehaviour> ItemsInbag => _itemsInBag;

    [SerializeField] private List<ItemBehaviour> _itemsInBag;

    private BagCell[] _bagCells;

    private void Awake()
    {
        _bagCells = GetComponentsInChildren<BagCell>();
    }

    public void AddItemToBag(ItemBehaviour item)
    {
        if (!_itemsInBag.Contains(item))
        {
            _itemsInBag.Add(item);
        }
    }
    public void RemoveItemFromBag(ItemBehaviour item)
    {
        if (_itemsInBag.Contains(item))
        {
            _itemsInBag.Remove(item);
        }
    }




}