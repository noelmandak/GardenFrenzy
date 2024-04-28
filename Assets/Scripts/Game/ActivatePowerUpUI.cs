using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ActivatePowerUpUI : MonoBehaviour
{
    public int currentPowerUpType;
    public string word;
    public float timer;


    public GameObject[] StarsGO;

    private SpeechRecord speechRecord;
    private PowerUpManager powerUpManager;

    public GameObject PopupRecording;
    public TextMeshProUGUI WordText;
    public TextMeshProUGUI EmotionText;
    public TextMeshProUGUI GetReadyTimerText;
    public GameObject GetReadyTimer;
    public GameObject RecordingParent;
    public Slider RecordingTimerSlider;
    public TextMeshProUGUI ProcessText;
    public GameObject PopupResult;
    public TextMeshProUGUI CommandText;
    public TextMeshProUGUI PredictedText;
    public TextMeshProUGUI DetailsText;

    public AudioSource source;

    private void Start()
    {
        speechRecord = GetComponentInParent<SpeechRecord>();
        powerUpManager = GetComponentInParent<PowerUpManager>();
    }

    public void OpenPopupRecording(string word, string emotion)
    {
        PopupRecording.SetActive(true);
        WordText.text = $"\"{word}\"";
        EmotionText.text = $"Emotion: {emotion}";
        GetReadyTimer.SetActive(true);
        StartCoroutine(StartGetReadyTimer());
    }

    public void SetGetReadyText(int countDown)
    {
        if (countDown > 0) GetReadyTimerText.text = $"Get Ready in {countDown}";
        else GetReadyTimerText.text = $"Get Ready in {countDown}";

    }

    private IEnumerator StartGetReadyTimer()
    {
        int countDown = 3;
        while (countDown > 0)
        {
            GetReadyTimerText.text = $"Get Ready in {countDown}";
            yield return new WaitForSeconds(1f); // Tunggu satu detik
            countDown--;
        }
        GetReadyTimer.SetActive(false);
        RecordingParent.SetActive(true);
        ProcessText.text = "Recording...";
        speechRecord.RecordFor3Seconds();

        StartCoroutine(MoveSlider());
    }

    public IEnumerator MoveSlider()
    {
        float timer = 0f;
        while (timer < 3f) // Atur sesuai dengan durasi merekam (3 detik)
        {
            float progress = timer / 3f;
            RecordingTimerSlider.value = progress * RecordingTimerSlider.maxValue;
            yield return null;
            timer += Time.deltaTime;
        }
        RecordingTimerSlider.value = RecordingTimerSlider.maxValue;


        ProcessText.text = "Processing...";
    }

    public void ShowResults(string targetWord, string targetEmotion, string predictedEmotion, float percentage, int star, string skill, int durationPU)
    {
        RecordingParent.SetActive(false);
        PopupRecording.SetActive(false);
        PopupResult.SetActive(true);
        CommandText.text = $"\"{targetWord}\" with {targetEmotion} Emotion";
        Debug.Log(CommandText.text);
        PredictedText.text = $"Predicted:\n{predictedEmotion} {percentage}%";

        SetStar(star);
        if (star > 0) DetailsText.text = $"Success activate {skill} for {durationPU}s";
        else DetailsText.text = $"Failed to Activate {skill}";
    }

    public void PlayRecordedSpeech()
    {
        source.PlayOneShot(speechRecord.clip);
    }

    public void CloseResults()
    {
        PopupResult.SetActive(false);
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
