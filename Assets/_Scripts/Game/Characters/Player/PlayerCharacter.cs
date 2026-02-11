using System;
using UnityEngine;


public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance { get; private set; }
    public event Action<int> OnTrophiesValueChanged;
    public event Action<int> OnLivesValueChanged;
    public event Action<ClassDataSO> OnPlayerClassChanged;

    [Header("Player specific")]
    [SerializeField] protected string RankName = string.Empty;
    [SerializeField] private int _lives;
    [SerializeField] private int _trophies;

    private ItemBehaviour _currentSelectedItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogError("More than one instance of player character");
            Destroy(gameObject);
            return;
        }

        character = Instance as PlayerCharacter;
    }


    //This is Start()
    protected override void InitializeCharacter()
    {
        base.InitializeCharacter();

        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
        SelectedItemManager.Instance.OnItemSelected += SelectedItemManager_OnItemSelected;
        SwitchClassButton.OnSwitchClassButtonPressed += SwitchClassButton_OnSwitchClassButtonPressed;
    }

    //This is OnDestroy()
    protected override void DestroyCharacter()
    {
        base.DestroyCharacter();
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        SelectedItemManager.Instance.OnItemSelected -= SelectedItemManager_OnItemSelected;
        SwitchClassButton.OnSwitchClassButtonPressed -= SwitchClassButton_OnSwitchClassButtonPressed;


        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.OnItemStateChanged -= SelectedItem_OnItemStateChanged;
        }

        //Static events
        try
        {
            SwitchClassButton.OnSwitchClassButtonPressed -= SwitchClassButton_OnSwitchClassButtonPressed;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error unsubscribing from static event: {e.Message}");
        }
    }
    private void SelectedItemManager_OnItemSelected(ItemBehaviour newItem)
    {
        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.OnItemStateChanged -= SelectedItem_OnItemStateChanged;
        }

        _currentSelectedItem = newItem;

        if (_currentSelectedItem != null)
        {
            _currentSelectedItem.OnItemStateChanged += SelectedItem_OnItemStateChanged;

            SelectedItem_OnItemStateChanged(_currentSelectedItem.PreviousState, _currentSelectedItem.CurrentState);
        }
    }

    private void SelectedItem_OnItemStateChanged(ItemBehaviour.ItemState previousState, ItemBehaviour.ItemState newState)
    {
        if (previousState.HasFlag(ItemBehaviour.ItemState.Store) && newState.HasFlag(ItemBehaviour.ItemState.Inventory))
        {
            BuyItem();
        }
    }

    private void BuyItem()
    {
        SpendMoney(_currentSelectedItem.GetItemPrice());
        _currentSelectedItem.SetItemState(ItemBehaviour.ItemState.Inventory);       
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


    private void SwitchClassButton_OnSwitchClassButtonPressed()
    {
        _currentClassIndex++;

        if (GameManager.Instance != null && 
            _currentClassIndex < GameManager.Instance.AllPlayableClasses.Length)
        {
            _classData = GameManager.Instance.AllPlayableClasses[_currentClassIndex];
            OnPlayerClassChanged?.Invoke(ClassData);
        }
        else
        {
            _currentClassIndex = 0;
            _classData = GameManager.Instance.AllPlayableClasses[_currentClassIndex];
            OnPlayerClassChanged?.Invoke(ClassData);
        }
    }

    public void SpendMoney(int moneyAmount)
    {

        if (Stats.GoldAmount - moneyAmount >= 0)
        {
            Stats.GoldAmount -= moneyAmount; 
            InvokeStatsChanged(_stats);
        }
        else
        {
            Debug.Log("You dont have money to do that");
        }

    }

    public void AddMoney(int moneyAmount)
    {
        if (Stats.GoldAmount + moneyAmount >= 0)
        {
            Stats.GoldAmount += moneyAmount;
            InvokeStatsChanged(_stats);
        }
        else
        {
            Debug.Log("You are trying to add negative money amount");

        }
    }

    public bool HasMoneyToBuyItem(int price)
    {
        return Stats.GoldAmount - price >= 0;
    }
    public void SellItem()
    {
        Stats.GoldAmount += (int)(_currentSelectedItem.GetItemPrice() / 2);
        InvokeStatsChanged(_stats);

        _currentSelectedItem.DestroySelf();
    }

    public string Rank => RankName;
    public int Lives => _lives;
    public int Trophies => _trophies;
}
