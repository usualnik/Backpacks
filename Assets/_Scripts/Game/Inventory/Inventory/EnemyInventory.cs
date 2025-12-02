using UnityEngine;

public class EnemyInventory : BaseInventory
{
    public static EnemyInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than one instance of Player Inventory");
        }
    }   
}
