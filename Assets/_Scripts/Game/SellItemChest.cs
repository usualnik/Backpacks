using UnityEngine;

public class SellItemChest : MonoBehaviour
{
    private ItemBehaviour _itemBehaviour;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour item))
        {
            _itemBehaviour = item;
            Invoke(nameof(DestroyObject), 1.0f);
        }
    }

    private void DestroyObject()
    {
        _itemBehaviour.gameObject.SetActive(false);

    }
}
