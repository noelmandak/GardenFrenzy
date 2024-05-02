using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;

public class PlayerHistoryManager : MonoBehaviour
{
    public Transform HistoryTableContainer;
    public GameObject RowTemplate;
    int templateHeight = 60;
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
                HistoryResponse response = JsonUtility.FromJson<HistoryResponse>(request.downloadHandler.text);
                CreateRow(response);
            }
        }
    }

    void CreateRow(HistoryResponse data)
    {
        RectTransform containerRect = HistoryTableContainer.GetComponent<RectTransform>();
        float newHeight = templateHeight * data.history.Count;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, newHeight);

        foreach (var historyData in data.history)
        {
            GameObject row = Instantiate(RowTemplate, HistoryTableContainer);
            row.GetComponent<PlayerHistoryRowComponent>().SetRowValue(historyData);
            row.SetActive(true);
        }
    }
}


[System.Serializable]
public class HistoryResponse
{
    public string message;
    public List<GameData> history = new List<GameData>();
}