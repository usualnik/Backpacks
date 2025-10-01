using UnityEngine;


public abstract class ItemDataSO : ScriptableObject
{
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public int Price => price;
    public ItemEffectSO[] Effects => effects;
    public GameObject Prefab => prefab;
    public ItemSubType SubType => itemSubType;
    public RarityType Rarity => rarity;

    public Vector2Int GetShapeSize() => new Vector2Int(shapeWidth, shapeHeight);

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
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject prefab;

    [Header("SubType")]
    [SerializeField] private ItemSubType itemSubType = ItemSubType.None;

    [Header("Rarity")]
    [SerializeField] private RarityType rarity = RarityType.None;

    [Header("Shape")]
    [SerializeField] private int shapeWidth = 1;
    [SerializeField] private int shapeHeight = 1;
    [SerializeField] private bool[] shapeArray;

    [Header("Effects")]
    [SerializeField] private ItemEffectSO[] effects;

    [Header("Properties")]
    [SerializeField] private int price;   
  

    public bool[,] GetShape()
    {
        bool[,] shape = new bool[shapeWidth, shapeHeight];
        if (shapeArray != null && shapeArray.Length == shapeWidth * shapeHeight)
        {
            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    shape[x, y] = shapeArray[y * shapeWidth + x];
                }
            }
        }
        return shape;
    }

    public void SetShape(bool[,] newShape)
    {
        shapeWidth = newShape.GetLength(0);
        shapeHeight = newShape.GetLength(1);
        shapeArray = new bool[shapeWidth * shapeHeight];

        for (int x = 0; x < shapeWidth; x++)
        {
            for (int y = 0; y < shapeHeight; y++)
            {
                shapeArray[y * shapeWidth + x] = newShape[x, y];
            }
        }
    }
   
}