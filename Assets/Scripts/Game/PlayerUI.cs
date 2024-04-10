using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    
    public GameObject playerRedKentangLogo;
    public GameObject playerRedWortelLogo;
    public GameObject playerBlueKentangLogo;
    public GameObject playerBlueWortelLogo;
    public TextMeshPro playerRedCounter;
    public TextMeshPro playerBlueCounter;

    public TextMeshProUGUI playerRedScoreText;
    public TextMeshProUGUI playerBlueScoreText;

    void Start()
    {
        playerRedKentangLogo.SetActive(false);
        playerRedWortelLogo.SetActive(false);
        playerBlueKentangLogo.SetActive(false);
        playerBlueWortelLogo.SetActive(false);
    }

    public void UpdateUI(bool isRed, int type, int counter)
    {
        if (isRed)
        {
            playerRedCounter.text = counter.ToString();
            switch (type)
            {
                case 0:
                    playerRedKentangLogo.SetActive(false);
                    playerRedWortelLogo.SetActive(false);
                    break;
                case 1:
                    playerRedKentangLogo.SetActive(true);
                    playerRedWortelLogo.SetActive(false);
                    break;
                case 2:
                    playerRedKentangLogo.SetActive(false);
                    playerRedWortelLogo.SetActive(true);
                    break;
            }
        } else
        {
            playerBlueCounter.text = counter.ToString();
            switch (type)
            {
                case 0:
                    playerBlueKentangLogo.SetActive(false);
                    playerBlueWortelLogo.SetActive(false);
                    break;
                case 1:
                    playerBlueKentangLogo.SetActive(true);
                    playerBlueWortelLogo.SetActive(false);
                    break;
                case 2:
                    playerBlueKentangLogo.SetActive(false);
                    playerBlueWortelLogo.SetActive(true);
                    break;
            }
        }
    }
    public void UpdateScore(int playerRedScore, int playerBlueScore)
    {
        playerRedScoreText.text = playerRedScore.ToString();
        playerBlueScoreText.text = playerBlueScore.ToString();
    }
}
