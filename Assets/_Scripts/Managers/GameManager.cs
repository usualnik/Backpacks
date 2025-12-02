using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int Round { get; private set; } = 1;

    public event Action<GameState> OnGameStateChanged;

    public ClassDataSO[] AllPlayableClasses => _allPlayableClasses;

    private const int MAX_ROUNDS_IN_GAME = 18;

    public enum GameState
    {
        Menu,
        Store,
        Gameplay
    }

    private GameState _state = GameState.Menu;

    [SerializeField] private ClassDataSO[] _allPlayableClasses;



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
        PlayerCharacter.Instance.OnLivesValueChanged += PlayerCharacter_OnLivesValueChanged;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        WindowManager.Instance.OnWindowOpened -= WindowManager_OnWindowOpened;
        PlayerCharacter.Instance.OnLivesValueChanged -= PlayerCharacter_OnLivesValueChanged;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }


    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {

        if (Round < MAX_ROUNDS_IN_GAME)
        {
            Round++;
        }
        else
        {
            //Restart game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }


    private void PlayerCharacter_OnLivesValueChanged(int playerLives)
    {
        if (playerLives <= 0)
        {
            //TODO: - reload game if lives less than 0
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //OnGameOver?.Invoke();
        }
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
