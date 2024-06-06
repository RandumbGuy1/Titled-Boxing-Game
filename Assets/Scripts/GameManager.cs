using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; } = GameState.Gameplay;
    public int CoinCount { get; set; } = 0;
    public Color GloveColor { get; set; } = Color.red;

    public delegate void GameStateChangeHandler(GameState newGameState);
    public event GameStateChangeHandler OnGameStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.SetState(GameState.Gameplay);
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetState(GameState.Gameplay);
        }
    }

    public void SetState(GameState newGameState)
    {
        if (newGameState == CurrentGameState) return;

        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(newGameState);
    }
}

public enum GameState
{
    Gameplay,
    Paused,
}
