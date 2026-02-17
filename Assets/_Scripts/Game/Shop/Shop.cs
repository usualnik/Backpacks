using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    public static Shop Instance { get; private set; }
    public event Action OnRerollPriceChanged;
    public event Action OnShopEnteredAfterCombat;

    [SerializeField] private Transform[] _shopTransforms;
    [SerializeField] private List<GameObject> _items = new List<GameObject>();

    [Header("Prices")]
    [SerializeField] private TextMeshProUGUI[] _priceTexts;
    [SerializeField] private GameObject[] _salePricesObjects;

    [Header("SYSTEM")]
    [SerializeField] private List<ItemDataSO> _allSpawnableItems;

    private const float SALE_CHANCE = 0.2f;
    private const int MAX_SPAWN_ATTEMPTS = 50; // Защита от бесконечного цикла

    private int _rerollPrice;
    private int _rerollProgression = 0;

    private const int REROLLS_NEEDED_TO_NEW_PRICE = 3;
    private const int INITIAL_REROLL_PRICE = 1;



    /// <summary>
    /// 
    /// Первый индекс это номер раунда, описаны раунды от 1 до 12,
    /// Начиная с 12 раунда и по последний 18 используется { 20,20,20,20,20 } 
    /// 
    /// Второй индекс описывает Шанс выпадения предмета, в зависимости от тира предмета 
    /// { Common, Rare, Epic, Legendary, Godly, Unique} 
    /// 
    /// </summary>
    /// 
    private int[,] _spawnChancesPerRound =
    {
        { 90,10,0,0,0,0 },
        { 84,15,1,0,0,0 },
        { 75,20,5,0,0,0 },
        { 64,25,10,1,0,0 },
        { 45,35,15,5,0,0 },
        { 29,40,20,10,1,0 },
        { 20,35,25,15,5,0 },
        { 20,30,25,15,10,0 },
        { 20,28,25,15,12,0 },
        { 20,25,25,15,15,0 },
        { 20,23,23,17,17,0 },
        { 20,20,20,20,20,0 }
    };

    private const int ROUND_TO_START_SPAWN_UNIQUE_ITEMS = 4;
    private const int UNIQUE_ITEM_SPAWN_CHANCE = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of shop manager");
    }

    private void Start()
    {
        SpawnItems();

        ResetRerollPrice();

        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        OnShopEnteredAfterCombat?.Invoke();

        // 1. Сначала сбрасываем цену реролла
        ResetRerollPrice();

        // 2. Спавним обычные предметы
        SpawnItems();

        // 3. Пытаемся заспавнить уникальный предмет (если подходит раунд)
        if (GameManager.Instance.Round >= ROUND_TO_START_SPAWN_UNIQUE_ITEMS)
        {
            CheckForSpawnUniqueItem();
        }
    }

    private void SpawnItems()
    {
        foreach (var salePriceObj in _salePricesObjects)
        {
            salePriceObj.SetActive(false);
        }

        for (int i = 0; i < _shopTransforms.Length; i++)
        {
            // Проверяем, есть ли в слоте ЗАРЕЗЕРВИРОВАННЫЙ предмет
            bool hasReservedItem = false;
            ShopItem[] shopItemsInSlot = _shopTransforms[i].GetComponentsInChildren<ShopItem>();

            foreach (ShopItem shopItem in shopItemsInSlot)
            {
                if (shopItem.GetIsItemReserved())
                {
                    hasReservedItem = true;
                    break;
                }
            }

            // Если есть зарезервированный предмет - пропускаем этот слот
            if (hasReservedItem)
            {
                continue;
            }

            // Очищаем слот от незарезервированных предметов
            if (shopItemsInSlot.Length > 0)
            {
                foreach (ShopItem shopItem in shopItemsInSlot)
                {
                    if (!shopItem.GetIsItemReserved())
                    {
                        _items.Remove(shopItem.gameObject);
                        Destroy(shopItem.gameObject);
                    }
                }
            }

            // Спавним новый предмет
            GameObject shopObject = SpawnRandomItemInSlot(i);
            if (shopObject != null)
            {
                _items.Add(shopObject);
            }
        }
    }

    private void CheckForSpawnUniqueItem()
    {
        bool isProcSpawnUniqueItem = UnityEngine.Random.Range(0, 100) <= UNIQUE_ITEM_SPAWN_CHANCE;

        if (isProcSpawnUniqueItem)
        {
            SpawnUniqueItemInRandomSlot();
        }
    }

    private void SpawnUniqueItemInRandomSlot()
    {
        // Получаем список уникальных предметов
        var allSpawnebleUniques = _allSpawnableItems
            .Where(i => i.Rarity == ItemDataSO.RarityType.Unique && i.IsSpawnableInShop)
            .ToList();

        if (allSpawnebleUniques.Count == 0)
        {
            Debug.LogWarning("No unique items available to spawn!");
            return;
        }

        // Выбираем случайный уникальный предмет
        int randomItemIndex = Random.Range(0, allSpawnebleUniques.Count);
        ItemDataSO uniqueItemData = allSpawnebleUniques[randomItemIndex];
        GameObject uniqueItemPrefab = uniqueItemData.Prefab;

        // Находим слот для спавна (исключая слоты с зарезервированными предметами)
        List<int> availableSlots = new List<int>();

        for (int i = 0; i < _shopTransforms.Length; i++)
        {
            bool hasReservedItem = false;
            ShopItem[] shopItemsInSlot = _shopTransforms[i].GetComponentsInChildren<ShopItem>();

            foreach (ShopItem shopItem in shopItemsInSlot)
            {
                if (shopItem.GetIsItemReserved())
                {
                    hasReservedItem = true;
                    break;
                }
            }

            if (!hasReservedItem)
            {
                availableSlots.Add(i);
            }
        }

        if (availableSlots.Count == 0)
        {
            Debug.LogWarning("No available slots for unique item!");
            return;
        }

        // Выбираем случайный доступный слот
        int randomSlotIndex = availableSlots[Random.Range(0, availableSlots.Count)];

        // Очищаем слот (удаляем обычный предмет, если есть)
        ShopItem[] existingItems = _shopTransforms[randomSlotIndex].GetComponentsInChildren<ShopItem>();
        foreach (ShopItem item in existingItems)
        {
            if (!item.GetIsItemReserved())
            {
                _items.Remove(item.gameObject);
                Destroy(item.gameObject);
            }
        }

        // Спавним уникальный предмет
        GameObject shopObject = Instantiate(uniqueItemPrefab, _shopTransforms[randomSlotIndex].position, Quaternion.identity);
        shopObject.transform.SetParent(_shopTransforms[randomSlotIndex], true);

        ItemBehaviour shopObjectItemBehaviour = shopObject.GetComponent<ItemBehaviour>();
        shopObjectItemBehaviour?.InitItemStateInStore();

        // Обрабатываем скидку
        bool itemOnSale = Random.value < SALE_CHANCE;

        if (itemOnSale)
        {
            int originalPrice = shopObjectItemBehaviour.GetItemPrice();
            int salePrice = Mathf.RoundToInt(originalPrice / 2f);

            _priceTexts[randomSlotIndex].text = originalPrice.ToString();
            _salePricesObjects[randomSlotIndex].SetActive(true);
            _salePricesObjects[randomSlotIndex].GetComponentInChildren<TextMeshProUGUI>().text = salePrice.ToString();

            shopObjectItemBehaviour.SetItemPrice(salePrice);
        }
        else
        {
            _priceTexts[randomSlotIndex].text = shopObjectItemBehaviour.GetItemPrice().ToString();
        }

        _items.Add(shopObject);
    }

    private GameObject SpawnRandomItemInSlot(int slotIndex)
    {
        int attempts = 0;

        // Определяем индекс для таблицы шансов
        int tableIndex;
        int currentRound = GameManager.Instance.Round;

        if (currentRound <= 12)
        {
            tableIndex = Mathf.Clamp(currentRound - 1, 0, 11);
        }
        else
        {
            tableIndex = 11;
        }

        while (attempts < MAX_SPAWN_ATTEMPTS)
        {
            attempts++;

            // 1. Выбираем случайное число для определения редкости
            int randomChance = Random.Range(1, 101);
            int selectedRarity = -1;
            int cumulativeChance = 0;

            // 2. Определяем редкость предмета на основе шансов для текущего раунда
            for (int rarity = 0; rarity < 6; rarity++)
            {
                cumulativeChance += _spawnChancesPerRound[tableIndex, rarity];
                if (randomChance <= cumulativeChance)
                {
                    selectedRarity = rarity;
                    break;
                }
            }

            // 3. Если не удалось определить редкость, используем последнюю
            if (selectedRarity == -1)
            {
                selectedRarity = 4; // Godly как fallback
            }

            // 4. Получаем все предметы выбранной редкости
            var itemsOfSelectedRarity = _allSpawnableItems
                .Where(item => (int)item.Rarity == selectedRarity &&
                              item.IsSpawnableInShop)
                .ToList();

            // 5. ЕСЛИ НЕТ ПРЕДМЕТОВ ВЫБРАННОЙ РЕДКОСТИ - ИЩЕМ ЛЮБУЮ ДОСТУПНУЮ РЕДКОСТЬ
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No items found for rarity {selectedRarity} in round {currentRound}. Searching for any available rarity...");

                // Ищем любую доступную редкость по порядку (от низшей к высшей)
                for (int fallbackRarity = 0; fallbackRarity < 6; fallbackRarity++)
                {
                    var fallbackItems = _allSpawnableItems
                        .Where(item => (int)item.Rarity == fallbackRarity &&
                                      item.IsSpawnableInShop)
                        .ToList();

                    if (fallbackItems.Count > 0)
                    {
                        itemsOfSelectedRarity = fallbackItems;
                        selectedRarity = fallbackRarity;
                        Debug.Log($"Using fallback rarity: {(ItemDataSO.RarityType)fallbackRarity}");
                        break;
                    }
                }
            }

            // 6. Если все еще нет предметов - пробуем еще раз
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No spawnable items available for any rarity. Attempt {attempts}/{MAX_SPAWN_ATTEMPTS}");
                continue;
            }

            // 7. Случайно выбираем предмет из нужной редкости
            int randomIndex = Random.Range(0, itemsOfSelectedRarity.Count);
            ItemDataSO selectedItemData = itemsOfSelectedRarity[randomIndex];
            GameObject itemPrefab = selectedItemData.Prefab;

            if (itemPrefab == null)
            {
                Debug.LogWarning($"Item prefab is null for item: {selectedItemData.name}");
                continue;
            }

            ShopItem newShopItem = itemPrefab.GetComponent<ShopItem>();
            if (newShopItem == null)
            {
                Debug.LogWarning($"ShopItem component is missing on prefab: {itemPrefab.name}");
                continue;
            }

            // 8. Спавним предмет
            GameObject shopObject = Instantiate(itemPrefab, _shopTransforms[slotIndex].position, Quaternion.identity);
            shopObject.transform.SetParent(_shopTransforms[slotIndex], true);

            ItemBehaviour shopObjectItemBehaviour = shopObject.GetComponent<ItemBehaviour>();
            shopObjectItemBehaviour?.InitItemStateInStore();

            // 9. Обрабатываем скидку
            bool itemOnSale = Random.value < SALE_CHANCE;

            if (itemOnSale)
            {
                int originalPrice = shopObjectItemBehaviour.GetItemPrice();
                int salePrice = Mathf.RoundToInt(originalPrice / 2f);

                _priceTexts[slotIndex].text = originalPrice.ToString();
                _salePricesObjects[slotIndex].SetActive(true);
                _salePricesObjects[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = salePrice.ToString();

                shopObjectItemBehaviour.SetItemPrice(salePrice);
            }
            else
            {
                _priceTexts[slotIndex].text = shopObjectItemBehaviour.GetItemPrice().ToString();
            }


            // 10. Указываем, что нужно инициализировать предмет как предмет игрока

            shopObject.GetComponent<OwnerTargetHandler>().SetPlayerItem(true);



            return shopObject;
        }

        Debug.LogError($"Failed to spawn item in slot {slotIndex} after {MAX_SPAWN_ATTEMPTS} attempts");
        return null;
    }

    public void Reroll()
    {
        _rerollProgression++;

        if (_rerollProgression % REROLLS_NEEDED_TO_NEW_PRICE == 0)
        {
            _rerollPrice++;
            OnRerollPriceChanged?.Invoke();
        }

        PlayerCharacter.Instance.SpendMoney(_rerollPrice);
        SpawnItems();
    }

    [ContextMenu("LOAD ALL ITEMS")]
    private void InitShopItems()
    {
        _allSpawnableItems = Resources.LoadAll<ItemDataSO>("ItemsData")
            .Where(item => item.IsSpawnableInShop && !item.Type.HasFlag(ItemDataSO.ItemType.Gemstone))
            .ToList();

        Debug.Log($"Loaded {_allSpawnableItems.Count} items");

        foreach (var itemData in _allSpawnableItems)
        {
            if (itemData.Prefab == null)
            {
                Debug.LogError($"Item {itemData.name} has null prefab!");
                continue;
            }

            ShopItem shopItem = itemData.Prefab.GetComponent<ShopItem>();
            if (shopItem == null)
            {
                Debug.LogError($"Item {itemData.name} prefab is missing ShopItem component!");
            }
        }
    }
    private void ResetRerollPrice()
    {
        _rerollPrice = INITIAL_REROLL_PRICE;
        OnRerollPriceChanged?.Invoke();
    }
    public void AddGemsToShop(List<ItemDataSO> gemsToAdd)
    {
        foreach (var gem in gemsToAdd)
        {
            if (!_allSpawnableItems.Contains(gem))
            {
                _allSpawnableItems.Add(gem);
            }
        }
    }
    public void RemoveGemsFromShop(List<ItemDataSO> gemsToRemove)
    {
        foreach (var gem in gemsToRemove)
        {
            if (_allSpawnableItems.Contains(gem))
            {
                _allSpawnableItems.Remove(gem);
            }
        }
    }
    public int GetCurrentRerollPrice() => _rerollPrice;
    public ItemDataSO GetRandomAvailableItemDataSO(Vector2Int itemShape)
    {
        int attempts = 0;

        // Определяем индекс для таблицы шансов
        int tableIndex;
        int currentRound = GameManager.Instance.Round;

        if (currentRound <= 12)
        {
            tableIndex = Mathf.Clamp(currentRound - 1, 0, 11);
        }
        else
        {
            tableIndex = 11;
        }

        while (attempts < MAX_SPAWN_ATTEMPTS)
        {
            attempts++;

            // 1. Выбираем случайное число для определения редкости
            int randomChance = Random.Range(1, 101);
            int selectedRarity = -1;
            int cumulativeChance = 0;

            // 2. Определяем редкость предмета на основе шансов для текущего раунда
            for (int rarity = 0; rarity < 6; rarity++)
            {
                cumulativeChance += _spawnChancesPerRound[tableIndex, rarity];
                if (randomChance <= cumulativeChance)
                {
                    selectedRarity = rarity;
                    break;
                }
            }

            // 3. Если не удалось определить редкость, используем последнюю
            if (selectedRarity == -1)
            {
                selectedRarity = 4; // Godly как fallback
            }

            // 4. Получаем все предметы выбранной редкости
            var itemsOfSelectedRarity = _allSpawnableItems
                .Where(item => (int)item.Rarity == selectedRarity &&
                              item.GetShapeSize() == itemShape && item.Type != ItemDataSO.ItemType.Bag)
                .ToList();

            // 5. ЕСЛИ НЕТ ПРЕДМЕТОВ ВЫБРАННОЙ РЕДКОСТИ - ИЩЕМ ЛЮБУЮ ДОСТУПНУЮ РЕДКОСТЬ
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No items found for rarity {selectedRarity} in round {currentRound}. Searching for any available rarity...");

                // Ищем любую доступную редкость по порядку (от низшей к высшей)
                for (int fallbackRarity = 0; fallbackRarity < 6; fallbackRarity++)
                {
                    var fallbackItems = _allSpawnableItems
                        .Where(item => (int)item.Rarity == fallbackRarity &&
                                      item.GetShapeSize() == itemShape && item.Type != ItemDataSO.ItemType.Bag)
                        .ToList();

                    if (fallbackItems.Count > 0)
                    {
                        itemsOfSelectedRarity = fallbackItems;
                        selectedRarity = fallbackRarity;
                        Debug.Log($"Using fallback rarity: {(ItemDataSO.RarityType)fallbackRarity}");
                        break;
                    }
                }
            }

            // 6. Если все еще нет предметов - пробуем еще раз
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No spawnable items available for any rarity. Attempt {attempts}/{MAX_SPAWN_ATTEMPTS}");
                continue;
            }

            // 7. Случайно выбираем предмет из нужной редкости
            int randomIndex = Random.Range(0, itemsOfSelectedRarity.Count);
            ItemDataSO selectedItemData = itemsOfSelectedRarity[randomIndex];

            return selectedItemData;
        }

        Debug.LogError($"Failed to find item after {MAX_SPAWN_ATTEMPTS} attempts");
        return null;

    }
    public ItemDataSO GetRandomAvailableItemDataSO()
    {
        int attempts = 0;

        // Определяем индекс для таблицы шансов
        int tableIndex;
        int currentRound = GameManager.Instance.Round;

        if (currentRound <= 12)
        {
            tableIndex = Mathf.Clamp(currentRound - 1, 0, 11);
        }
        else
        {
            tableIndex = 11;
        }

        while (attempts < MAX_SPAWN_ATTEMPTS)
        {
            attempts++;

            // 1. Выбираем случайное число для определения редкости
            int randomChance = Random.Range(1, 101);
            int selectedRarity = -1;
            int cumulativeChance = 0;

            // 2. Определяем редкость предмета на основе шансов для текущего раунда
            for (int rarity = 0; rarity < 6; rarity++)
            {
                cumulativeChance += _spawnChancesPerRound[tableIndex, rarity];
                if (randomChance <= cumulativeChance)
                {
                    selectedRarity = rarity;
                    break;
                }
            }

            // 3. Если не удалось определить редкость, используем последнюю
            if (selectedRarity == -1)
            {
                selectedRarity = 4; // Godly как fallback
            }

            // 4. Получаем все предметы выбранной редкости
            var itemsOfSelectedRarity = _allSpawnableItems
                .Where(item => (int)item.Rarity == selectedRarity && item.Type != ItemDataSO.ItemType.Bag)
                .ToList();

            // 5. ЕСЛИ НЕТ ПРЕДМЕТОВ ВЫБРАННОЙ РЕДКОСТИ - ИЩЕМ ЛЮБУЮ ДОСТУПНУЮ РЕДКОСТЬ
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No items found for rarity {selectedRarity} in round {currentRound}. Searching for any available rarity...");

                // Ищем любую доступную редкость по порядку (от низшей к высшей)
                for (int fallbackRarity = 0; fallbackRarity < 6; fallbackRarity++)
                {
                    var fallbackItems = _allSpawnableItems
                        .Where(item => (int)item.Rarity == fallbackRarity && item.Type != ItemDataSO.ItemType.Bag)
                        .ToList();

                    if (fallbackItems.Count > 0)
                    {
                        itemsOfSelectedRarity = fallbackItems;
                        selectedRarity = fallbackRarity;
                        Debug.Log($"Using fallback rarity: {(ItemDataSO.RarityType)fallbackRarity}");
                        break;
                    }
                }
            }

            // 6. Если все еще нет предметов - пробуем еще раз
            if (itemsOfSelectedRarity.Count == 0)
            {
                Debug.LogWarning($"No spawnable items available for any rarity. Attempt {attempts}/{MAX_SPAWN_ATTEMPTS}");
                continue;
            }

            // 7. Случайно выбираем предмет из нужной редкости
            int randomIndex = Random.Range(0, itemsOfSelectedRarity.Count);
            ItemDataSO selectedItemData = itemsOfSelectedRarity[randomIndex];

            return selectedItemData;
        }

        Debug.LogError($"Failed to find item after {MAX_SPAWN_ATTEMPTS} attempts");
        return null;

    }
    public float GetCheapestBagGearScoreAmount()
    {
        ItemDataSO leatherBag = _allSpawnableItems.Where(i => i.ItemName == "LeatherBag").FirstOrDefault();
        return leatherBag.GearScore;
       
    }
}