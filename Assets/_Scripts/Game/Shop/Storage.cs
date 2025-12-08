using UnityEngine;

public class Storage : MonoBehaviour
{
    public static Storage Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("More than one instance of Storage");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour item))
        {
           item.SetItemState(ItemBehaviour.ItemState.Storage);
        }
    }


}
