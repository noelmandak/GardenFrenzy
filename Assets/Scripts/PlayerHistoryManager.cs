using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerHistoryManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetPlayerHistory());
    }

    IEnumerator GetPlayerHistory()
    {

        string user_id = PlayerPrefs.GetString("UserId", "");
        string url = "https://lizard-alive-suitably.ngrok-free.app/history/" + user_id;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Parse JSON response
                HistoryResponse response = JsonUtility.FromJson<HistoryResponse>(request.downloadHandler.text);
                // Now you can access the data in the response like this:
                Debug.Log("Message: " + response.message);
                Debug.Log(request.downloadHandler.text);

                //foreach (var historyData in response.history)
                //{
                //    Debug.Log("History ID: " + historyData.history_id);
                //    Debug.Log("Duration: " + historyData.duration);
                //    Debug.Log("Score: " + historyData.score);
                //    Debug.Log("Timeplay: " + historyData.time_play);
                //    Debug.Log("User ID: " + historyData.user_id);

                //    foreach (var emotionData in historyData.emotion_history)
                //    {
                //        Debug.Log("Emotion Target: " + emotionData.emotion_target);
                //        Debug.Log("Percentage: " + emotionData.percentage);
                //        Debug.Log("Timestamp: " + emotionData.time_stamp);
                //        Debug.Log("Voice Emotion: " + emotionData.voice_emotion);
                //        Debug.Log("Word: " + emotionData.word);
                //    }
                //}
            }
        }
    }
}


[System.Serializable]
public class HistoryResponse
{
    public string message;
    public List<GameData> history = new List<GameData>();
}