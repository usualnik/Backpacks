using UnityEngine;


public abstract class ItemDataSO : ScriptableObject
{
    public string ItemName => _itemName;
    public Sprite Icon => _icon;
    public int Price => _price;
    public ItemEffectSO Effect => _effect;
    public GameObject Prefab => _prefab;
    public ItemSubType SubType => _itemSubType;
    public RarityType Rarity => _rarity;

    public Vector2Int GetShapeSize() => new Vector2Int(_shapeWidth, _shapeHeight);

    public enum ItemType
    {
        None,
        Accessory,
        Armor,
        Gems,
        Food,
        Potions,
        PlayingCards,
        MeleeWeapons,
        RangedWeapons,
        Pets,
        Bags,
        Shields,
    }

    public enum ItemSubType
    {
        None,
        Dark,
        Holy,
        Magical,
        Natural,
        Vampire,
    }

    public enum RarityType
    {
        None,
        Common,
        Rare,
        Epic,
        Legendary,
        Divine,
        Unique,
    }


    [Header("Basic Info")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _prefab;

    [Header("SubType")]
    [SerializeField] private ItemSubType _itemSubType = ItemSubType.None;

    [Header("Rarity")]
    [SerializeField] private RarityType _rarity = RarityType.None;

    [Header("Shape")]
    [SerializeField] private int _shapeWidth = 1;
    [SerializeField] private int _shapeHeight = 1;
    [SerializeField] private bool[] _shapeArray;

    [Header("Effects")]
    [SerializeField] private ItemEffectSO _effect;

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


    public abstract void PerformAction(ItemBehaviour.Target target);  
   
}