using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    public enum GameState
    {
        Menu,
        Store,
        Gameplay
    }

    private GameState _state = GameState.Menu;
     

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Debug.LogError("More than one instance of game manager");
    }
    

    private void Start()
    {
        WindowManager.Instance.OnWindowOpened += WindowManager_OnWindowOpened;
    }    

    private void OnDestroy()
    {
        WindowManager.Instance.OnWindowOpened += WindowManager_OnWindowOpened;
    }

    private void WindowManager_OnWindowOpened(WindowManager.WindowType windowType)
    {
        switch (windowType)
        {
            case WindowManager.WindowType.MainMenu:
                ChangeGameState(GameState.Menu);
                break;
            case WindowManager.WindowType.Store:
                ChangeGameState(GameState.Store);

                break;
            case WindowManager.WindowType.Gameplay:
                ChangeGameState(GameState.Gameplay);
                break;
            default:
                break;
        }
    }

    private void ChangeGameState(GameState gameState)
    {
        _state = gameState;
        OnGameStateChanged?.Invoke(_state);
    }

    private GameState GetGameState() => _state;
}
