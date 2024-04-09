using Google.Protobuf.WellKnownTypes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PowerUpUI powerUpUI;
    private List<PowerUpClass> activePowerUps = new();
    public GameObject[] RedPUEffect;
    public GameObject[] BluePUEffect;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void ActivatePower(bool isPlayerRed, int powerUpType, int star)
    {
        float duration = GetDuration(star);

        PowerUpClass powerUp = new(powerUpType, duration, isPlayerRed);
        activePowerUps.Add(powerUp);

        ApplyPowerUpEffect(isPlayerRed, powerUpType);
        StartCoroutine(RemovePowerUpAfterDuration(powerUp));
    }
    private IEnumerator RemovePowerUpAfterDuration(PowerUpClass powerUp)
    {
        Debug.Log("masuk sini");
        while (!gameManager.IsPaused())         {
            yield return new WaitForSeconds(powerUp.duration);
            
            RemovePowerUpFromPlayer(powerUp.isRed, powerUp.powerUpType);
            break;
        }
    }

    private void Update()
    {
        List<int> RedPowerUpType = new List<int>();
        List<float> RedPowerUpDuration = new List<float>();
        List<int> BluePowerUpType = new List<int>();
        List<float> BluePowerUpDuration  = new List<float>();

        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            if (!gameManager.IsPaused()) 
            {
                activePowerUps[i].UpdateDuration(Time.deltaTime);
                if (activePowerUps[i].isRed) 
                {
                    RedPowerUpType.Add(activePowerUps[i].powerUpType);
                    RedPowerUpDuration.Add((int)activePowerUps[i].GetDuration());
                } 
                else 
                {
                    BluePowerUpType.Add(activePowerUps[i].powerUpType);
                    BluePowerUpDuration.Add((int)activePowerUps[i].GetDuration());
                }
            }

            if (activePowerUps[i].IsExpired())
            {
                activePowerUps.RemoveAt(i);
            }

        }
        powerUpUI.UpdateMessage(true, RedPowerUpType.ToArray(), RedPowerUpDuration.ToArray());
        powerUpUI.UpdateMessage(false, BluePowerUpType.ToArray(), BluePowerUpDuration.ToArray());
    }

    private float GetDuration(int star)
    {
        return star switch
        {
            1 => 8.0f,
            2 => 15.0f,
            3 => 20.0f,
            _ => 0.0f,
        };
    }

    private void ApplyPowerUpEffect(bool isPlayerRed, int powerUpType)
    {
        Debug.Log($"Acctivate {isPlayerRed} {powerUpType}");
        if (powerUpType == 1)
        {
            float speedMultiplier = 2f;
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetPlayerSpeed(speedMultiplier);
        }else if (powerUpType == 2)
        {
            float speedMultiplier = 1/5f;
            (isPlayerRed ? gameManager.GetPlayerBlue() : gameManager.GetPlayerRed()).SetPlayerSpeed(speedMultiplier);
        }else if (powerUpType == 3)
        {
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetFearField(true);
        }
        else if (powerUpType == 4)
        {
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetDoublePoints(true);
        }
        SetPUEffect(isPlayerRed,powerUpType,true);
    }
    private void RemovePowerUpFromPlayer(bool isPlayerRed, int powerUpType)
    {
        Debug.Log($"Remove {isPlayerRed} {powerUpType}");
        if (powerUpType == 1)
        {
            float speedMultiplier = 1/2f;
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetPlayerSpeed(speedMultiplier);
        }
        else if (powerUpType == 2)
        {
            float speedMultiplier = 5f;
            (isPlayerRed ? gameManager.GetPlayerBlue() : gameManager.GetPlayerRed()).SetPlayerSpeed(speedMultiplier);
        }
        else if (powerUpType == 3)
        {
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetFearField(false);
        }
        else if (powerUpType == 4)
        {
            (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetDoublePoints(false);
        }
        SetPUEffect(isPlayerRed, powerUpType, false);
    }

    public void SetPUEffect(bool isPlayerRed, int powerUpType, bool active)
    {
        if (isPlayerRed) RedPUEffect[powerUpType - 1].SetActive(active);
        else BluePUEffect[powerUpType - 1].SetActive(active);
    }
}


public class PowerUpClass
{
    public bool isRed;
    public int powerUpType;
    public float duration;

    public PowerUpClass(int type, float duration, bool isRed)
    {
        this.powerUpType = type;
        this.duration = duration;
        this.isRed = isRed;
    }

    public void UpdateDuration(float deltaTime)
    {
        duration -= deltaTime;
    }

    public float GetDuration()
    {
        return duration;
    }
    public bool IsExpired()
    {
        return duration <= 0;
    }
}