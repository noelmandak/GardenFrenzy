using Google.Protobuf.WellKnownTypes;
using System;
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
    private SpeechRecord speechRecord;

    public ActivatePowerUpUI activatePowerUpUI;
    private bool isCurrentPlayerIsRed;


    public string targetWord;
    public string targetEmotion;
    public string predictedEmotion = "";
    public float percentage = 0f;
    public int star = 0;
    public int durationPU = 0;
    public string skill = "";
    public int currentPowerUpType;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        speechRecord = GetComponent<SpeechRecord>();
    }

    public void StartActivatePowerUp(bool isPlayerRed, string word, int powerupType)
    {
        targetWord = word;
        isCurrentPlayerIsRed = isPlayerRed;
        currentPowerUpType = powerupType;
        switch (powerupType)
        {
            case 1:
                targetEmotion  = "Angry";
                skill = "speed up self movement";
                break;
            case 2:
                targetEmotion = "Sad";
                skill = "slow down opponent movement";
                break;
            case 3:
                targetEmotion = "Fear";
                skill = "fear field";
                break;
            case 4:
                targetEmotion = "Joy";
                skill = "double points";
                break;

        }
        Debug.Log("hehe");
        Debug.Log(activatePowerUpUI);

        activatePowerUpUI.OpenPopupRecording(word, targetEmotion);
    }

    public void ProcessResults()
    {
        star = GetStar(percentage);
        if (targetEmotion != predictedEmotion) star = 0;
        durationPU = (int)GetDuration(star);
        activatePowerUpUI.ShowResults(targetWord, targetEmotion, predictedEmotion, percentage, star, skill, durationPU);

        DateTime timeStamp = DateTime.Now;

        EmotionHistory emotionData = new EmotionHistory();
        emotionData.word = targetWord;
        emotionData.emotion_target = targetEmotion;
        emotionData.voice_emotion = predictedEmotion;
        emotionData.percentage = percentage;
        emotionData.time_stamp = timeStamp;


        gameManager.emotionList.Add(emotionData);
        if (star > 0) ApplyPowerUp(isCurrentPlayerIsRed, currentPowerUpType, star);
    }


    public int GetStar(float emotionScore)
    {
        if (emotionScore >= 90 && emotionScore <= 100) return 3;
        else if (emotionScore >= 50 && emotionScore <= 89) return 2;
        else if (emotionScore >= 10 && emotionScore <= 49) return 1;
        else if (emotionScore >= 0 && emotionScore <= 9) return 0;
        else
        {
            Debug.LogError("Nilai emosi di luar rentang yang diharapkan.");
            return -1;
        }
    }



    private IEnumerator StartGetReadyTimer()
    {
        int countDown = 3;
        while (countDown > 0)
        {
            activatePowerUpUI.SetGetReadyText(countDown);
            yield return new WaitForSeconds(1f); // Tunggu satu detik
            countDown--;
        }
        activatePowerUpUI.SetGetReadyText(countDown);
        yield return new WaitForSeconds(1f); // Tunggu satu detik lagi sebelum merekam

        activatePowerUpUI.GetReadyTimer.SetActive(false);
        activatePowerUpUI.RecordingParent.SetActive(true);
        activatePowerUpUI.ProcessText.text = "Recording...";
        speechRecord.RecordFor3Seconds();

        StartCoroutine(activatePowerUpUI.MoveSlider());
    }

    public void ApplyPowerUp(bool isPlayerRed, int powerUpType, int star)
    {
        float duration = GetDuration(star);

        PowerUpClass powerUp = new(powerUpType, duration, isPlayerRed);
        activePowerUps.Add(powerUp);

        ApplyPowerUpEffect(isPlayerRed, powerUpType);
        StartCoroutine(RemovePowerUpAfterDuration(powerUp));
    }
    private IEnumerator RemovePowerUpAfterDuration(PowerUpClass powerUp)
    {
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
        if (powerUpType == 1) (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).ResetSpeed();
        if (powerUpType == 2) (isPlayerRed ? gameManager.GetPlayerBlue() : gameManager.GetPlayerRed()).ResetSpeed();
        if (powerUpType == 3) (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetFearField(false);
        if (powerUpType == 4) (isPlayerRed ? gameManager.GetPlayerRed() : gameManager.GetPlayerBlue()).SetDoublePoints(false);
        SetPUEffect(isPlayerRed, powerUpType, false);
    }

    public void SetPUEffect(bool isPlayerRed, int powerUpType, bool active)
    {
        if (isPlayerRed) RedPUEffect[powerUpType - 1].SetActive(active);
        else BluePUEffect[powerUpType - 1].SetActive(active);
    }
    public void ResetPowerUps()
    {
        foreach (PowerUpClass powerUp in activePowerUps)
        {
            RemovePowerUpFromPlayer(powerUp.isRed, powerUp.powerUpType);
        }
        activePowerUps.Clear();
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