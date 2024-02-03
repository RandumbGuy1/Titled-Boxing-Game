using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerRef player;
    private bool paused = false;

    void Awake() => player.PlayerInput.OnPauseToggle += HandlePause;
    void OnDestroy() => player.PlayerInput.OnPauseToggle -= HandlePause;

    private void HandlePause(bool pause)
    {
        if (!pause) return;

        paused = !paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        GameManager.Instance.SetState(paused ? GameState.Paused : GameState.Gameplay);
        pauseMenu.SetActive(paused);
    }

    public void UnPause()
    {
        HandlePause(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleEditor()
    {
        HandlePause(true);

        GameManager.Instance.SetState(GameState.Gameplay);
        pauseMenu.SetActive(false);
    }
}
