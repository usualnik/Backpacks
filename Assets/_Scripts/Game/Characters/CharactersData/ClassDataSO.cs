using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "ClassDataSO")]
public class ClassDataSO : ScriptableObject
{
    public string ClassName => _className;
    public Sprite ClassSprite => _classSprite;
    public ClassType Class => _class;

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

}
