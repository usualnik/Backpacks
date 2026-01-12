using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : BaseInventory
{
    public static EnemyInventory Instance { get; private set; }

    [SerializeField] private List<ItemBehaviour> _itemsInIventory;
    [SerializeField] private List<BagPreset> _leatherBagPreset;

    private float _targetGearScore;
    private const float BAGS_GEARSCORE_PERCENTAGE = 30f; // % который должны занимать сумки в целевом гирскоре врага.
    private float _cheapestBagGearScoreAmount = 0.8f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than one instance of Enemy Inventory");
        }
    }

    private void Start()
    {
        EnemyCharacter.Instance.OnEnemyGenerated += EnemyCharacter_OnEnemyGenerated;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

        _cheapestBagGearScoreAmount = Shop.Instance.GetCheapestBagGearScoreAmount();
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
        SpawnItems();
    }
    private void SpawnItems()
    {
        _targetGearScore = PlayerInventory.Instance.GetPlayerGearScore();

        SpawnStartingBag();

        // Вычисляем сколько МОЖНО потратить на сумки
        float gearScoreAvailableForBags = _targetGearScore * BAGS_GEARSCORE_PERCENTAGE / 100;

        float leatherBagGearScore = _leatherBagPreset[0].GetComponent<ItemBehaviour>().ItemData.GearScore;

        int bagsAvailable = _leatherBagPreset.Capacity - 1;


        while (gearScoreAvailableForBags > _cheapestBagGearScoreAmount && bagsAvailable > 0)
        {
            _leatherBagPreset[bagsAvailable].gameObject.SetActive(true);

            // Вычитаем из обоих значений
            _targetGearScore -= leatherBagGearScore;
            gearScoreAvailableForBags -= leatherBagGearScore;

            _itemsInIventory.Add(_leatherBagPreset[bagsAvailable].GetComponent<ItemBehaviour>());

            ConfigureItemsInBag(_leatherBagPreset[bagsAvailable]);

            bagsAvailable--;

        }

    }
    private void SpawnStartingBag()
    {
        ClassDataSO enemyClass = EnemyCharacter.Instance.ClassData;

        GameObject startingBag = Instantiate(enemyClass.StartingUniquebag, transform);
        ItemBehaviour startingBagBehaviour = startingBag.GetComponent<ItemBehaviour>();

        if (startingBagBehaviour == null)
        {
            return;
        }

        startingBagBehaviour.InitItemStateInInventory();

        _itemsInIventory.Add(startingBagBehaviour);

        BagPreset startingBagPreset = startingBag.AddComponent<BagPreset>();

        startingBagPreset.InitPresetedItemsInBag();

        ConfigureItemsInBag(startingBagPreset);

        float startingBagGearScore = startingBagBehaviour.ItemData.GearScore;

        _targetGearScore -= startingBagGearScore;

    }

    /*  
     * Запросил у магазина предмет с  формой и кол-вом ячеек как у этого предмета
     * Удалил пресет, заспавнил на его месте новый предмет
     * Уменьшил гирскор бюджет
     * Если гирскор закончился - выходим из спавна
     * 
     */

    private void ConfigureItemsInBag(BagPreset bag)
    {
        foreach (var item in bag.PresetedItems)
        {
            ItemDataSO newItem = Shop.Instance.GetRandomAvailableItemDataSO(item.GetComponent<ItemBehaviour>().ItemData.GetShapeSize());

            GameObject spawnedItem = Instantiate(newItem.Prefab, item.transform.position, item.transform.rotation , bag.transform);

            spawnedItem.GetComponent<ItemBehaviour>().InitItemStateInInventory();

            _targetGearScore -= newItem.GearScore;

            Destroy(item.gameObject);
        }

        bag.ClearPresetList();
    }

    private void ClearEnemyInventoryAfterCombat()
    {
        foreach (var item in _itemsInIventory)
        {
            Destroy(item.gameObject);
        }

        _itemsInIventory.Clear();
        _targetGearScore = 0;
    }
}
