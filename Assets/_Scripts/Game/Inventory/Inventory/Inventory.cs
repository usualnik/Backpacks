using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _rangerStartItemsConfig;
    [SerializeField] private GameObject _reaperStartItemsConfig;

    private List<GameObject> _configs = new List<GameObject>();

    private void Awake()
    {
        _configs.Add(_rangerStartItemsConfig);
        _configs.Add(_reaperStartItemsConfig);
    }

    private void Start()
    {
        InitStartItems();
        PlayerCharacter.Instance.OnPlayerClassChanged += PlayerCharacter_OnPlayerClassChanged;
    }
    private void OnDestroy()
    {
        PlayerCharacter.Instance.OnPlayerClassChanged -= PlayerCharacter_OnPlayerClassChanged;

    }
    private void PlayerCharacter_OnPlayerClassChanged(ClassDataSO newPlayerClass)
    {
        UpdateItemsConfig(newPlayerClass.Class);
    }

    private void InitStartItems()
    {
        switch (PlayerCharacter.Instance.ClassData.Class)
        {
            case ClassDataSO.ClassType.Ranger:
                _rangerStartItemsConfig.gameObject.SetActive(true);
                break;
            case ClassDataSO.ClassType.Reaper:
                _reaperStartItemsConfig.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
    private void UpdateItemsConfig(ClassDataSO.ClassType newPlayerClass)
    {
        foreach (var item in _configs)
        {
            item.gameObject.SetActive(false);
        }

        switch (newPlayerClass)
        {
            case ClassDataSO.ClassType.Ranger:
                _rangerStartItemsConfig.SetActive(true);
                break;
            case ClassDataSO.ClassType.Reaper:
                _reaperStartItemsConfig.SetActive(true);
                break;
            default:
                break;
        }
    }
}
