using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedSettingsUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    
    public TextMeshProUGUI changePlayerButtonText;

    public PlayerController playerController;
    public Slider playerSpeedSlider;


    private void Start()
    {
        speedText.text = "Speed: " + playerController.GetPlayerSpeed().ToString();
        playerSpeedSlider.value = playerController.GetPlayerSpeed();
        playerSpeedSlider.onValueChanged.AddListener((float value) => playerController.SetPlayerSpeed(value));
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + playerController.GetPlayerSpeed().ToString();
        changePlayerButtonText.text = "Play " + (playerController.isRed ? "Blue" : "Red");
    }
}
