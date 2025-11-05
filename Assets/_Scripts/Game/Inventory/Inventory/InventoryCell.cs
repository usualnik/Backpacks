using UnityEngine;
using UnityEngine.UI;

public class InventoryCell : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    public DraggableBag OccupyingBag { get; private set; }

    private void OnDestroy()
    {
        if (IsOccupied && OccupyingBag != null)
        {
            SetOccupied(false, null);
        }
    }

    public void SetOccupied(bool occupied, DraggableBag bag)
    {
        IsOccupied = occupied;
        OccupyingBag = bag;

        //// Визуальная обратная связь
        //Image image = GetComponent<Image>();
        //if (image != null)
        //{
        //    image.color = occupied ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        //}
    }

}
