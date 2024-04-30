using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlaySFX("gameover");
        AudioManager.Instance.StopMusic();

        scoreText.text = PlayerPrefs.GetString("LastScore");
    }

}
