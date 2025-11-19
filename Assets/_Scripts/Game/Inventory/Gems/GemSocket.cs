using UnityEngine;

public class GemSocket : MonoBehaviour
{
    public ItemBehaviour Item => _item;
    public bool CanBePlaced { get; private set; }
    public bool IsOccupied { get; private set; }
    public DraggableGem OccupyingGem { get; private set; }


    private ItemBehaviour _item;


    private void Awake()
    {
        _item = GetComponentInParent<ItemBehaviour>();
    }

    private void OnDestroy()
    {
        if (IsOccupied && OccupyingGem != null)
        {
            SetOccupied(false, OccupyingGem);
        }
    }

    public void SetOccupied(bool occupied, DraggableGem item)
    {
        if (occupied)
        {
            IsOccupied = true;
            OccupyingGem = item;
        }
        else
        {
            IsOccupied = false;
            OccupyingGem = null;
        }
    }

   
}
