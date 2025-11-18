using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    protected Image _itemCellImage;
    public bool CanBePlaced { get; protected set; }
    public BagCell CurrentBagCell { get; protected set; }

    private void Awake()
    {
        _itemCellImage = GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTriggerEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTriggerExit(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
       HandleTriggerStay(collision);
    }


    protected virtual void HandleTriggerEnter(Collider2D collision)
    {
        if (collision.TryGetComponent(out BagCell bagCell) && !bagCell.IsOccupied)
        {
            CanBePlaced = true;
            CurrentBagCell = bagCell;
            _itemCellImage.color = new Color(0, 1, 0, 0.5f);
        }
    }
    protected virtual void HandleTriggerExit(Collider2D collision)
    {
        if (collision.TryGetComponent(out BagCell bagCell))
        {
            CanBePlaced = false;
            CurrentBagCell = null;
            _itemCellImage.color = new Color(1, 0, 0, 0.5f);
        }
    }
    protected virtual void HandleTriggerStay(Collider2D collision)
    {
        if (collision.TryGetComponent(out BagCell bagCell))
        {
            if (!bagCell.IsOccupied || bagCell.OccupyingItem == GetComponentInParent<DraggableItem>())
            {
                CanBePlaced = true;
                CurrentBagCell = bagCell;
                _itemCellImage.color = new Color(0, 1, 0, 0.5f);
            }
            else
            {
                CanBePlaced = false;
                CurrentBagCell = null;
                _itemCellImage.color = new Color(1, 0, 0, 0.5f);
            }
        }
    }
}