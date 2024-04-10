using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivatePowerUp : MonoBehaviour
{

    public int currentPowerUpType;
    public TextMeshProUGUI commandText;
    public GameObject[] StarsGO;

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

    public void SetStar(int star)
    {
        switch (star)
        {
            case 0:
                StarsGO[0].SetActive(false); 
                StarsGO[1].SetActive(false); 
                StarsGO[2].SetActive(false); 
                break;
            case 1:
                StarsGO[0].SetActive(true); 
                StarsGO[1].SetActive(false); 
                StarsGO[2].SetActive(false); 
                break;
            case 2:
                StarsGO[0].SetActive(true); 
                StarsGO[1].SetActive(true); 
                StarsGO[2].SetActive(false); 
                break;
            case 3:
                StarsGO[0].SetActive(true); 
                StarsGO[1].SetActive(true); 
                StarsGO[2].SetActive(true); 
                break;
        }
    }
}
