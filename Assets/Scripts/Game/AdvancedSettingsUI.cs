using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedSettingsUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    
    public TextMeshProUGUI changePlayerButtonText;

    private GameManager gameManager;
    public Slider playerSpeedSlider;


    private void Start()
    {
        speedText.text = "Speed: " + gameManager.GetCurrentPlayer().PlayerSpeed.ToString();
        playerSpeedSlider.value = gameManager.GetCurrentPlayer().PlayerSpeed;
        playerSpeedSlider.onValueChanged.AddListener((float value) => gameManager.GetCurrentPlayer().PlayerSpeed = value);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(gameManager.GetCurrentPlayer().PlayerSpeed.ToString());
        speedText.text = "Speed: " + gameManager.GetCurrentPlayer().PlayerSpeed.ToString();
        changePlayerButtonText.text = "Play " + (gameManager.isCurretPlayerIsRed ? "Blue" : "Red");
    }
}
