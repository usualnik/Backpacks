using System;
using UnityEngine;


public abstract class ItemDataSO : ScriptableObject
{
    public string ItemName => _itemName;
    public Sprite Icon => _icon;
    public int Price => _price;
    public GameObject Prefab => _prefab;
    public ExtraType ItemExtraType => _itemExtraType;
    public RarityType Rarity => _rarity;
    public bool IsSpawnableInShop => _isSpawnableInShop;
    public ItemType Type => _itemType;
    public ItemDataSO[] RecipeIngridients => _recipeIngridients;
    public ItemDataSO RecipeResult => _recipeResult;
    public Vector2Int GetShapeSize() => new Vector2Int(_shapeWidth, _shapeHeight);
    public float GearScore => _gearScore;

    [Flags]
    public enum ItemType
    {
        None            = 0,
        Accessory       = 1 << 0,  // 1
        Armor           = 1 << 1,  // 2
        Gemstone        = 1 << 2,  // 4
        Food            = 1 << 3,  // 8
        Potion          = 1 << 4,  // 16
        PlayingCard     = 1 << 5,  // 32
        Weapon          = 1 << 6,  // 64
        Pet             = 1 << 7,  // 128
        Bag             = 1 << 8,  // 256
        Shield          = 1 << 9,  // 512
        Helmet          = 1 << 10, // 1024  
        Gloves          = 1 << 11, // 2048
        Shoes           = 1 << 12, // 4096
        Skill           = 1 << 13, // 8192              
        SpellScroll     = 1 << 14, // 16384
        Book            = 1 << 15, // 32768
        Chess           = 1 << 16, // 65536
    }

    [Flags]
    public enum ExtraType
    {
        None        = 0,
        Melee       = 1 << 0,       // 1
        Ranged      = 1 << 1,       // 2      
        Effect      = 1 << 2,       // 4
        Nature      = 1 << 3,       // 8
        Magic       = 1 << 4,       // 16
        Holy        = 1 << 5,       // 32
        Dark        = 1 << 6,       // 64
        Vampiric    = 1 << 7,       // 128
        Fire        = 1 << 8,       // 256
        Ice         = 1 << 9,       // 512
        Musical     = 1 << 10,      // 1024
    }

    public enum RarityType
    {
        Common = 0,      
        Rare = 1,        // 1  
        Epic = 2,        // 2
        Legendary = 3,   // 3
        Godly = 4,       // 4
        Unique = 5       // 5
    }


    [Header("Basic Info")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private bool _isSpawnableInShop;

    [Header("SubType")]
    [SerializeField] private ItemType _itemType = ItemType.None;
    [SerializeField] private ExtraType _itemExtraType = ExtraType.None;

    [Header("Rarity")]
    [SerializeField] private RarityType _rarity = RarityType.Common;

    [Header("Shape")]
    [SerializeField] private int _shapeWidth = 1;
    [SerializeField] private int _shapeHeight = 1;
    [SerializeField] private bool[] _shapeArray;

    [Header("Recipe")]
    [SerializeField] private ItemDataSO[] _recipeIngridients;
    [SerializeField] private ItemDataSO _recipeResult;

    [Header("Properties")]
    [SerializeField] private int _price;
    [SerializeField] private float _gearScore = 0.0f; 

    public bool[,] GetShape()
    {
        bool[,] shape = new bool[_shapeWidth, _shapeHeight];
        if (_shapeArray != null && _shapeArray.Length == _shapeWidth * _shapeHeight)
        {
            for (int x = 0; x < _shapeWidth; x++)
            {
                for (int y = 0; y < _shapeHeight; y++)
                {
                    shape[x, y] = _shapeArray[y * _shapeWidth + x];
                }
            }
        }
        return shape;
    }

    public void SetShape(bool[,] newShape)
    {
        _shapeWidth = newShape.GetLength(0);
        _shapeHeight = newShape.GetLength(1);
        _shapeArray = new bool[_shapeWidth * _shapeHeight];

        for (int x = 0; x < _shapeWidth; x++)
        {
            for (int y = 0; y < _shapeHeight; y++)
            {
                _shapeArray[y * _shapeWidth + x] = newShape[x, y];
            }
        }
    }
}