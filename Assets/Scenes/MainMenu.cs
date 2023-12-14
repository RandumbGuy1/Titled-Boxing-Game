using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] MenuSway menuSway;
    [SerializeField] GameObject main;
    [SerializeField] GameObject settings;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GoToSettingsMenu()
    {
        menuSway.InSettings = !menuSway.InSettings;
        settings.SetActive(!settings.activeSelf);
        main.SetActive(!main.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
