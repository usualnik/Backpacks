using System;
using UnityEngine;

public class EnemyCharacter : Character
{
    public static EnemyCharacter Instance { get; private set; }
    public event Action<ClassDataSO> OnEnemyGenerated;
    
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

        Shop.Instance.OnShopEnteredAfterCombat += Shop_OnShopEnteredAfterCombat;

        GenerateEnemy();
    }

    //This is OnDestroy()
    protected override void DestroyCharacter()
    {
        base.DestroyCharacter();

        Shop.Instance.OnShopEnteredAfterCombat -= Shop_OnShopEnteredAfterCombat;

    }

    private void Shop_OnShopEnteredAfterCombat()
    {
        GenerateEnemy();
    }

    private void GenerateEnemy()
    {
        int randomEnemyIndex = UnityEngine.Random.Range(0, GameManager.Instance.AllPlayableClasses.Length);

        _currentClassIndex = randomEnemyIndex;

        if (GameManager.Instance != null)
        {
            _classData = GameManager.Instance.AllPlayableClasses[randomEnemyIndex];
            OnEnemyGenerated?.Invoke(ClassData);
        }
    }

}
