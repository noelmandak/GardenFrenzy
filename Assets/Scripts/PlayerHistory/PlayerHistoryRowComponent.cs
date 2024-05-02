using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHistoryRowComponent : MonoBehaviour
{
    public Text Data;
    public GameObject[] Emotions;

    public void SetRowValue(GameData gameData)
    {
        Debug.Log(gameData.timeplay);
        DateTime parsedTime = DateTime.ParseExact(gameData.timeplay, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
        Debug.Log($"{parsedTime}  {gameData.timeplay}");
        // Mengonversi objek DateTime ke string dengan format yang diinginkan
        string formattedTime = parsedTime.ToString("dd/MM/yy HH:mm");
        Data.text = $": {gameData.score}\n: {gameData.duration} s\n: {formattedTime}";
        for (int i = 0; i < gameData.emotion_history.Count; i++)
        {
            EmotionHistory emotionHistory = gameData.emotion_history[i];
            int emotionType = 0;
            switch (emotionHistory.emotion_target)
            {
                case "Angry": emotionType = 1; break;
                case "Sad": emotionType = 2; break;
                case "Fear": emotionType = 3; break;
                case "Joy": emotionType = 4; break;
            }
            int star = (emotionHistory.emotion_target == emotionHistory.voice_emotion) ? GetStar(emotionHistory.percentage) : 0;

            EmotionComponent emotionComponent =  Emotions[emotionType-1].GetComponent<EmotionComponent>();
            emotionComponent.SetEmotionLogo(emotionType);
            emotionComponent.SetStars(star);
        }
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
}
