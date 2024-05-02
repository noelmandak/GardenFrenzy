using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRowComponent : MonoBehaviour
{
    public Text Index;
    public Text Username;
    public Text Score;

    public void SetRowValue(ScoreRow scoreData, int index)
    {
        Index.text = index.ToString();
        Username.text = scoreData.username;
        Score.text = scoreData.highscore.ToString();

    }
}
