using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] MenuSway menuSway;
    [SerializeField] GameObject[] menus;

    public void PlayGame(int index)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    }

    public void GoToMenu(int index)
    {
        for (int i = 0; i < menus.Length; i++) menus[i].SetActive(i == index);

        menuSway.MenuState = (MainMenuState) index;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum MainMenuState
{
    Main = 0,
    Settings = 1,
    LevelSelect = 2,
}
