using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsControls : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private TextMeshProUGUI volumeTextUI = null; 

    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    void Start ()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions(); //clears out all options of resolutions

        List<string> options = new List<string>(); // create list of options 

        int currentResolutionIndex = 0;
        for (int i =0; i < resolutions.Length; i++) // go through each resolution option and add them 
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
    }
    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution= resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }
    public void VolumeSlider(float volume)
    {
        volumeTextUI.text = volume.ToString("0.0");
    }
    public void SaveVolumeButton()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("VolumeValue", volumeValue);
        LoadValues();
    }
    void LoadValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("VolumeValue");
        volumeSlider.value = volumeValue;
        AudioListener.volume = volumeValue;
    }
    public void SetQuality (int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
