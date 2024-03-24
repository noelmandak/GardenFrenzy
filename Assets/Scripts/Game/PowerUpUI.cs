using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpUI : MonoBehaviour
{
    public GameObject[] PowerUpButtons = new GameObject[3];

    public void UpdatePowerUpButton(int[] playerPowerUps)
    {
        for (int i = 0; i < 3; i++)
        {
            int powerUpType = playerPowerUps[i];
            GameObject powerUpButtons = PowerUpButtons[i];
            switch (powerUpType)
            {
                case 0:
                    // Nonaktifkan semua game object child
                    foreach (Transform child in powerUpButtons.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                    break;
                case 1:
                    // Aktifkan Red game object
                    ActivatePowerUpWithTag(powerUpButtons.transform, "Red");
                    break;
                case 2:
                    // Aktifkan Blue game object
                    ActivatePowerUpWithTag(powerUpButtons.transform, "Blue");
                    break;
                case 3:
                    // Aktifkan Purple game object
                    ActivatePowerUpWithTag(powerUpButtons.transform, "Purple");
                    break;
                case 4:
                    // Aktifkan Yellow game object
                    ActivatePowerUpWithTag(powerUpButtons.transform, "Yellow");
                    break;
            }
        }
    }

    void ActivatePowerUpWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
