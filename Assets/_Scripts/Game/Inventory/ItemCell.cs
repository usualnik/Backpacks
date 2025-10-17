using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    private Image _itemCellImage;
    public bool CanBePlaced { get; private set; }
    public BagCell CurrentBagCell { get; private set; }

    private void Awake()
    {
        _itemCellImage = GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BagCell bagCell) && !bagCell.IsOccupied)
        {
            CanBePlaced = true;
            CurrentBagCell = bagCell;
            _itemCellImage.color = new Color(0, 1, 0, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BagCell bagCell))
        {
            CanBePlaced = false;
            CurrentBagCell = null;
            _itemCellImage.color = new Color(1, 0, 0, 0.5f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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