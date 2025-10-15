using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public event Action<int> OnLevelChanged;

    private int _levelIndex = 0;  
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of level manager");
    }
    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += Instance_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= Instance_OnCombatFinished;

    }

    private void Instance_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        _levelIndex++;
        OnLevelChanged?.Invoke(_levelIndex);
    }
   

    public int GetLevelIndex() => _levelIndex;
}
