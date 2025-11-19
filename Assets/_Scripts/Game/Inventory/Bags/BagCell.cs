using UnityEngine;
using UnityEngine.UI;

public class BagCell : MonoBehaviour
{
    public ItemBehaviour BagItem => _bagItem;
    public bool CanBePlaced { get; private set; }
    public InventoryCell CurrentInventoryCell { get; private set; }
    public bool IsOccupied { get; private set; }
    public DraggableItem OccupyingItem { get; private set; }


    private Color _canbePlacedColor = Color.green;
    private Color _normalColor;
    private Bag _parentBag;
    private ItemBehaviour _bagItem;
    private Image _bagCellImage;



    private void Awake()
    {
        _bagCellImage = GetComponent<Image>();
        _parentBag = GetComponentInParent<Bag>();
        _bagItem = GetComponentInParent<ItemBehaviour>();

        _normalColor = _bagCellImage.color;
    }

    private void OnDestroy()
    {
        if (IsOccupied && OccupyingItem != null)
        {
            SetOccupied(false, OccupyingItem);
        }
    }

    public void SetOccupied(bool occupied, DraggableItem item)
    {
        if (occupied)
        {
            _parentBag.AddItemToBag(item.ItemBehaviour);

            IsOccupied = true;
            OccupyingItem = item;
        }
        else
        {
            _parentBag.RemoveItemFromBag(item.ItemBehaviour);

            IsOccupied = false;
            OccupyingItem = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryCell inventoryCell) && !inventoryCell.IsOccupied)
        {
            CanBePlaced = true;
            CurrentInventoryCell = inventoryCell;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryCell inventoryCell))
        {
            CanBePlaced = false;
            CurrentInventoryCell = null;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out InventoryCell InventoryCell))
        {
            if (!InventoryCell.IsOccupied || InventoryCell.OccupyingBag == GetComponentInParent<DraggableBag>())
            {
                CanBePlaced = true;
                CurrentInventoryCell = InventoryCell;
                //_bagCellImage.color = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                CanBePlaced = false;
                CurrentInventoryCell = null;
                //_bagCellImage.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }


}