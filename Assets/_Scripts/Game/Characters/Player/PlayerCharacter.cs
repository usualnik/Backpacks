using System;
using UnityEngine;


public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance {  get; private set; }
    public event Action<int> OnTrophiesValueChanged;
    public event Action<int> OnLivesValueChanged;


    [Header("Player specific")]
    [SerializeField] protected string RankName = string.Empty;
    [SerializeField] private int _lives;
    [SerializeField] private int _trophies;

    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("More than one instance of player character");       
    }

    
    //This is Start()
    protected override void InitializeCharacter()
    {
        base.InitializeCharacter();

        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;


    }

 
    //This is OnDestroy()
    protected override void DestroyCharacter()
    {
        base.DestroyCharacter();
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult combatResult)
    {
        if (combatResult == CombatManager.CombatResult.PlayerWin)
        {
            _trophies++;
            OnTrophiesValueChanged?.Invoke(_trophies);
        }
        else if (combatResult == CombatManager.CombatResult.EnemyWin)
        {
            _lives--;
            OnLivesValueChanged?.Invoke(_lives);
        }

    }


    public string Rank => RankName;
    public int Lives => _lives;
    public int Trophies => _trophies;
}
