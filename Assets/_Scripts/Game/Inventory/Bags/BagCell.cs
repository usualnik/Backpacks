using UnityEngine;
using UnityEngine.UI;

public class BagCell : MonoBehaviour
{
    private Image _bagCellImage;
    public bool CanBePlaced { get; private set; }
    public InventoryCell CurrentInventoryCell { get; private set; }
    public bool IsOccupied { get; private set; }
    public DraggableItem OccupyingItem { get; private set; }


    private Color _canbePlacedColor = Color.green;
    private Color _normalColor;





    private void Awake()
    {
        _bagCellImage = GetComponent<Image>();

        _normalColor = _bagCellImage.color;
    }

    private void OnDestroy()
    {
        if (IsOccupied && OccupyingItem != null)
        {
            SetOccupied(false, null);
        }
    }

    public void SetOccupied(bool occupied, DraggableItem item)
    {
        IsOccupied = occupied;
        OccupyingItem = item;

        //// Визуальная обратная связь
        //Image image = GetComponent<Image>();
        //if (image != null)
        //{
        //    image.color = occupied ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        //}
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