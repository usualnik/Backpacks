using UnityEngine;

public class SellItemChest : MonoBehaviour
{
    private ItemBehaviour _itemBehaviour;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour item))
        {
            if (item.CurrentState.HasFlag(ItemBehaviour.ItemState.Inventory) ||
                item.CurrentState.HasFlag(ItemBehaviour.ItemState.Storage))
                item.GetComponent<DraggableItem>().SetCanBeSelled();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour item))
        {
            if (item.CurrentState.HasFlag(ItemBehaviour.ItemState.Inventory) ||
                item.CurrentState.HasFlag(ItemBehaviour.ItemState.Storage))
                item.GetComponent<DraggableItem>().SetCanBeSelled();
        }
    }



}
