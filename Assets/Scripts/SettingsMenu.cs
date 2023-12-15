using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject videoMenu;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject controlsMenu;

    [Header("Refrences")]
    [SerializeField] private PlayerRef player;
    private List<GameObject> menus = new List<GameObject>();

    void Awake()
    {
        menus = new List<GameObject>
        {
            videoMenu,
            audioMenu,
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

    public void ToggleIdleSway(bool isOn)
    {
        player.CameraBody.CamIdleSway.Enable(isOn);
    }

    public void ToggleHeadBob(bool isOn)
    {
        player.CameraBody.CamHeadBob.Enable(isOn);
    }

    public void SetVideoMenu()
    {
        videoMenu.SetActive(!videoMenu.activeInHierarchy);
        CloseMenus(videoMenu);
    }

    public void SetAudioMenu()
    {
        audioMenu.SetActive(!audioMenu.activeInHierarchy);
        CloseMenus(audioMenu);
    }

    public void SetControlsMenu()
    {
        controlsMenu.SetActive(!controlsMenu.activeInHierarchy);
        CloseMenus(controlsMenu);
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
