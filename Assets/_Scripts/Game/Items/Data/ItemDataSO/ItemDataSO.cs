using System;
using UnityEngine;


public abstract class ItemDataSO : ScriptableObject
{
    public string ItemName => _itemName;
    public Sprite Icon => _icon;
    public int Price => _price;
    public GameObject Prefab => _prefab;
    public ItemSubType SubType => _itemSubType;
    public RarityType Rarity => _rarity;
    public bool IsSpawnableInShop => _isSpawnableInShop;
    public ItemType Type => _itemType;
    public ItemDataSO[] RecipeIngridients => _recipeIngridients;
    public ItemDataSO RecipeResult => _recipeResult;
    public Vector2Int GetShapeSize() => new Vector2Int(_shapeWidth, _shapeHeight);

    [Flags]
    public enum ItemType
    {
        None            = 0,
        Accessory       = 1 << 0,  // 1
        Armor           = 1 << 1,  // 2
        Gems            = 1 << 2,  // 4
        Food            = 1 << 3,  // 8
        Potions         = 1 << 4,  // 16
        PlayingCards    = 1 << 5,  // 32
        MeleeWeapons    = 1 << 6,  // 64
        RangedWeapons   = 1 << 7,  // 128
        Pets            = 1 << 8,  // 256
        Bags            = 1 << 9,  // 512
        Shields         = 1 << 10, // 1024
        Helmets         = 1 << 11, // 2048  
        Gloves          = 1 << 12, // 4096 
        Boots           = 1 << 13, // 8192        
    }

    [Flags]
    public enum ItemSubType
    {
        None        = 0,
        Dark        = 1 << 0,   // 1
        Holy        = 1 << 1,   // 2
        Magical     = 1 << 2,   // 4
        Natural     = 1 << 3,   // 8
        Vampire     = 1 << 4,   // 16
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
    [SerializeField] private ItemSubType _itemSubType = ItemSubType.None;

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