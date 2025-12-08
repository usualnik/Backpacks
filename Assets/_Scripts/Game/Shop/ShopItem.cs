using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool _isItemReserved;
    [SerializeField] private bool _isSpawnableInShop;

    private ItemBehaviour _itemBehaviour;
    private ReservedImage _reservedImage;

    private void Awake()
    {
        _itemBehaviour = GetComponent<ItemBehaviour>();
        
    }
    private void Start()
    {
        _reservedImage = GetComponentInChildren<ReservedImage>(true);
        _isSpawnableInShop = _itemBehaviour.ItemData.IsSpawnableInShop;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (_itemBehaviour.CurrentState.HasFlag(ItemBehaviour.ItemState.Store))
            {
                _isItemReserved = _isItemReserved ? false : true;
                _reservedImage.gameObject.SetActive(_isItemReserved);
            }          
        }
    }

    public bool GetCanBeSpawnedInShop() => _itemBehaviour.ItemData.IsSpawnableInShop;
    public bool GetIsItemReserved() => _isItemReserved;
    public bool IsSpawnable => _isSpawnableInShop;

    public void SetSpawnableInShop(bool isSpawnableInShop)
    {
        _isSpawnableInShop = isSpawnableInShop;
    }
}
