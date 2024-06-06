using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public Toggle ShowIntroductionToggle;

    void Start()
    {
        if (PlayerPrefs.HasKey("ShowIntroduction"))
        {
            string showIntroduction = PlayerPrefs.GetString("ShowIntroduction");
            ShowIntroductionToggle.isOn = showIntroduction == "true";
        }

        ShowIntroductionToggle.onValueChanged.AddListener(delegate {
            onShowIntroductionToggleChange(ShowIntroductionToggle.isOn);
        });
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSFX()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        AudioManager.Instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    } 
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
    public void onShowIntroductionToggleChange(bool isOn)
    {
        string valueToSave = isOn ? "true" : "false";
        PlayerPrefs.SetString("ShowIntroduction", valueToSave);
        PlayerPrefs.Save();
    }
}
