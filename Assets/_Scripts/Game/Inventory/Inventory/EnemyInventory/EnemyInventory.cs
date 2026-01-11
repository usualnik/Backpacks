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

        ClassDataSO enemyClass = EnemyCharacter.Instance.ClassData;

        // Первой спавним стартовую сумку класса
        GameObject startingBag = Instantiate(enemyClass.StartingUniquebag, transform);
        ItemBehaviour startingBagBehaviour = startingBag.GetComponent<ItemBehaviour>();

        if (startingBagBehaviour != null)
        {
            startingBagBehaviour.InitItemStateInInventory();
            _itemsInIventory.Add(startingBagBehaviour);
        }

        float startingBagGearScore = startingBagBehaviour.ItemData.GearScore;

        _targetGearScore -= startingBagGearScore;

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

            //ConfigureItemsInBag(_leatherBagPreset[bagsAvailable]);

            bagsAvailable--;

        }

    }


    /*  
     * Запросил у магазина предмет с  формой и кол-вом ячеек как у этого предмета
     * Удалил пресет, заспавнил на его месте новый предмет
     * Уменьшил гирскор бюджет
     * Если гирскор закончился - выходим из спавна
     * 
     */

    //private void ConfigureItemsInBag(BagPreset bag)
    //{
    //    GameObject spawnedItem;
    //    RectTransform pos;

    //    foreach (var item in bag.PresetedItems)
    //    {

    //    } 
    //}

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
