using UnityEngine;

public class EnemyInventory : BaseInventory
{
    public static EnemyInventory Instance { get; private set; }

    [SerializeField]
    private InventoryCell[] _inventoryCells;

    private int _itemsInInventory;

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

    private void Start()
    {
        InitEnemyInventory();

        EnemyCharacter.Instance.OnEnemyGenerated += EnemyCharacter_OnEnemyGenerated;
    }


    private void OnDestroy()
    {
        EnemyCharacter.Instance.OnEnemyGenerated -= EnemyCharacter_OnEnemyGenerated;
    }

    private void EnemyCharacter_OnEnemyGenerated(ClassDataSO generatedEnemy)
    {
        GenerateEnemyInventory();                                                              
    }

    private void InitEnemyInventory()
    {

    } 

    private void GenerateEnemyInventory()
    {
       // 
    }
}
