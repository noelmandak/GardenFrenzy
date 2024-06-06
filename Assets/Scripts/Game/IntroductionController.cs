using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroductionController : MonoBehaviour
{
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public Toggle ShowIntroductionToggle;
    int currentPage = 0;

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




    public void PrevButtonOnClick()
    {
        currentPage = (currentPage - 1 + 3) % 3;
        ShowPage();
    }
    public void NextButtonOnClick()
    {
        currentPage = (currentPage + 1) % 3;
        ShowPage();
    }

    private void ShowPage()
    {
        switch (currentPage)
        {
            case 0:
                page1.SetActive(true);
                page2.SetActive(false);
                page3.SetActive(false);
                break;
            case 1:
                page1.SetActive(false);
                page2.SetActive(true);
                page3.SetActive(false);
                break;
            case 2:
                page1.SetActive(false);
                page2.SetActive(false);
                page3.SetActive(true);
                break;
        }
    }
    public void PlayExampleSound(string soundName)
    {
        AudioManager.Instance.PlaySFX(soundName);
    }
    public void PlayAngryExampleSound()
    {
        AudioManager.Instance.PlaySFX("angry-example");
    }
    public void PlaySadExampleSound()
    {
        AudioManager.Instance.PlaySFX("sad-example");
    }
    public void PlayFearExampleSound()
    {
        AudioManager.Instance.PlaySFX("fear-example");
    }
    public void PlayJoyExampleSound()
    {
        AudioManager.Instance.PlaySFX("joy-example");
    }

    public void onShowIntroductionToggleChange(bool isOn)
    {
        string valueToSave = isOn ? "true" : "false";
        PlayerPrefs.SetString("ShowIntroduction", valueToSave);
        PlayerPrefs.Save();
    }
}
