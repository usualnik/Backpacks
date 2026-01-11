using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "ClassDataSO")]
public class ClassDataSO : ScriptableObject
{
    public string ClassName => _className;
    public Sprite ClassSprite => _classSprite;
    public ClassType Class => _class;
    public GameObject StartingUniquebag => _startingUniqueBag;
    public GameObject LeatherBag => _startingItems[0];

    [SerializeField] private string _className;
    [SerializeField] private Sprite _classSprite;

    public enum ClassType
    {
        Ranger,
        Reaper
    }

    [SerializeField] private ClassType _class;
     
    [Header("StartingItems")]

    [SerializeField] private GameObject _startingUniqueBag;
    [SerializeField] private GameObject[] _startingItems;

    public List<ItemBehaviour> GetAllStartItems()
    {
        List<ItemBehaviour> startItems = new List<ItemBehaviour>();

        startItems.Add(_startingUniqueBag.GetComponent<ItemBehaviour>());

        foreach (var item in _startingItems)
        {
            startItems.Add(item.GetComponent<ItemBehaviour>());
        }

        return startItems;
    }

}
