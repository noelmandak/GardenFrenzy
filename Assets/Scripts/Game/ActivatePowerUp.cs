using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivatePowerUp : MonoBehaviour
{

    public int currentPowerUpType;
    public TextMeshProUGUI commandText;

    public void SetCommand()
    {
        string emotion = "";
        switch (currentPowerUpType)
        {
            case 1: 
                emotion = "Angry";
                break;
            case 2: 
                emotion = "Sad";
                break;
            case 3: 
                emotion = "Fear";
                break;
            case 4: 
                emotion = "Joy";
                break;

        }
        commandText.text = $"\"Ayam\" with {emotion} Emotion";
    }
}
