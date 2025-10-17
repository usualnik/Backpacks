using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBag : DraggableItem
{
    [SerializeField] private BagCell[] bagCells;

    //protected override void CheckPutInSlot(PointerEventData eventData)
    //{
    //    if (eventData.pointerCurrentRaycast.gameObject != null &&
    //        eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out InventoryCell cell))
    //    {
    //        transform.position = cell.transform.position;
    //        _isDragging = false;
    //    }
    //    else
    //    {
    //        _rb.bodyType = RigidbodyType2D.Dynamic;
    //        _isDragging = false;
    //    }
    //}
}
