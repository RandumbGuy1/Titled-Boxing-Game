using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject controlsMenu;

    [Header("Refrences")]
    private List<GameObject> menus = new List<GameObject>();

    void Awake()
    {
        menus = new List<GameObject>
        {
            settingsMenu,
            controlsMenu,
        };
 
        GameManager.Instance.OnGameStateChanged += PauseEnable;
    }

    void OnDestroy() => GameManager.Instance.OnGameStateChanged -= PauseEnable;

    private void PauseEnable(GameState newState)
    {
        if (newState == GameState.Paused) return;
        CloseMenus();
    }

    public void SetTargetFrameRate(string frames)
    {
        int newFrames = Mathf.Clamp(System.Convert.ToInt32(frames), 1, 200);
        Application.targetFrameRate = newFrames;
    }

    public void SetFullScreen(bool isOn)
    {
        Screen.fullScreen = isOn;
    }

    public void SetMenu(int i)
    {
        menus[i].SetActive(true);
        CloseMenus(menus[i]);
    }

    public void CloseMenus(GameObject avoid = null)
    {
        foreach (GameObject menu in menus)
        {
            if (menu == avoid) continue;
            menu.SetActive(false);
        }
    }
}
