using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyInventory : BaseInventory
{
    public static EnemyInventory Instance { get; private set; }

    [SerializeField] private InventoryCell[] _inventoryCells;
    [SerializeField] private List<ItemBehaviour> _itemsInIventory;

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
        EnemyCharacter.Instance.OnEnemyGenerated += EnemyCharacter_OnEnemyGenerated;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        EnemyCharacter.Instance.OnEnemyGenerated -= EnemyCharacter_OnEnemyGenerated;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        ClearEnemyInventoryAfterCombat();
    }
    private void EnemyCharacter_OnEnemyGenerated(ClassDataSO generatedEnemy)
    {
        GenerateEnemyInventory();
    }

    private void GenerateEnemyInventory()
    {
        // 1. Задаем целевой гир скор 
        float targetGearScore = PlayerInventory.Instance.GetPlayerGearScore();
        float currentGearScore = 0;

        // 2. Выбираем предметы из доступных на данном уровне 
        while (currentGearScore < targetGearScore)
        {
            ItemDataSO item = Shop.Instance.GetRandomAvailableItemDataSO();
            GameObject itemPrefab = item.Prefab;

            if (itemPrefab != null)
            {
                // Инстанциируем предмет
                GameObject spawnedItem = Instantiate(itemPrefab, transform);
                currentGearScore += item.GearScore;

                // Получаем ItemBehaviour у инстанциированного объекта
                ItemBehaviour itemBehaviour = spawnedItem.GetComponent<ItemBehaviour>();

                // 3. Инициализация предметов
                if (itemBehaviour != null)
                {
                    itemBehaviour.InitItemStateInInventory();
                    _itemsInIventory.Add(itemBehaviour);
                }

                // TODO: Предметы в инвентаре врага нельзя перетаскивать
                // Заглушка, переделать на интерфейс с возможностью отключения перетаскивания

                if (spawnedItem.GetComponent<DraggableItem>())
                {
                    spawnedItem.GetComponent<DraggableItem>().enabled = false;

                }else if (spawnedItem.GetComponent<DraggableBag>())
                {
                    spawnedItem.GetComponent<DraggableBag>().enabled = false;
                }
                else if (spawnedItem.GetComponent<DraggableGem>())
                {
                    spawnedItem.GetComponent<DraggableGem>().enabled = false;
                }

                // 4. Расставляем предметы

            }
        }
    }

    private void ClearEnemyInventoryAfterCombat()
    {
        foreach (var item in _itemsInIventory)
        {
            Destroy(item.gameObject);
        }

        _itemsInIventory.Clear();
    }
}
