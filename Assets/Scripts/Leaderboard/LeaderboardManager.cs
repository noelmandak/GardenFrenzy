using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class LeaderboardManager : MonoBehaviour
{
    public Transform ScoreTableContainer;
    public GameObject RowTemplate;
    int templateHeight = 40;
    void Start()
    {
        StartCoroutine(GetPlayerHistory());
    }

    IEnumerator GetPlayerHistory()
    {
        string url = "https://lizard-alive-suitably.ngrok-free.app/history/high_score";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);
                CreateRow(response);
            }
        }
    }

    void CreateRow(LeaderboardResponse data)
    {
        RectTransform containerRect = ScoreTableContainer.GetComponent<RectTransform>();
        float newHeight = templateHeight * data.highscore_data.Count;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, newHeight);

        int i = 0;
        foreach (var scoreData in data.highscore_data)
        {
            i++;
            GameObject row = Instantiate(RowTemplate, ScoreTableContainer);
            row.GetComponent<ScoreRowComponent>().SetRowValue(scoreData, i);
            row.SetActive(true);
        }
    }
}


[System.Serializable]
public class LeaderboardResponse
{
    public string message;
    public List<ScoreRow> highscore_data = new List<ScoreRow>();
}


[System.Serializable]
public class ScoreRow
{
    public string username;
    public int highscore;
}