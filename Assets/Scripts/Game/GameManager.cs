using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject pausePopup;
    public GameObject activatePowerPopup;
    public GameObject advancedSettingsPopup;
    public ActivatePowerUpUI activatePowerUp;
    public GameObject gameCanvas;
    public Slider timerSlider;

    private float playerSpeed = 5;
    private int maxCapacity = 5;
    public int totalPotato = 5;
    public int totalCarot = 5;
    private float duration = 100f;
    private float timer;
    public bool isPaused { get; private set; }
    private bool isActivatingPower = false;
    private bool isAdvancedSettings = false;

    private PowerUpManager powerUpManager;
    private VegetableSpawner vegetableSpawner;
    private PowerUpSpawner powerUpSpawner;

    private PlayerUI playerUI;
    private PowerUpUI powerUpUI;


    private Player currentPlayer;
    public bool isCurretPlayerIsRed = true;
    public bool isDisplayUI = true;
    public bool isTraining;


    [SerializeField]
    private InputActionReference move_action;
    public Player playerRed;
    public Player playerBlue;
    public RLAgent AgentRed;
    public RLAgent AgentBlue;
    public CameraFollow cameraFollow;

    private DateTime startTime;
    private DateTime endTime;

    private List<string> friendlyWords = new List<string> {
        "Hello", "Smile", "Love", "Happy", "Friend",
        "Cheerful", "Kindness", "Joy", "Gratitude", "Sunshine"
    };

    public List<EmotionHistory> emotionList = new List<EmotionHistory>();

    public string GetRandomWord()
    {
        // Generate a random index
        int randomIndex = UnityEngine.Random.Range(0, friendlyWords.Count);

        // Get the word at the random index
        string randomWord = friendlyWords[randomIndex];

        return randomWord;
    }
    private void Start()
    {
        powerUpManager = gameObject.GetComponent<PowerUpManager>();
        vegetableSpawner = gameObject.GetComponent<VegetableSpawner>();
        powerUpSpawner = gameObject.GetComponent<PowerUpSpawner>();
        playerUI = gameObject.GetComponent<PlayerUI>();
        powerUpUI = gameObject.GetComponent<PowerUpUI>();

        isPaused = false;
        isActivatingPower = false;
        isAdvancedSettings = false;
        if (!isDisplayUI) gameCanvas.SetActive(false);

        timerSlider.maxValue = duration;
        timer = duration;

        playerRed.Init(playerSpeed, maxCapacity);
        playerBlue.Init(playerSpeed, maxCapacity);
        vegetableSpawner.Init(totalPotato, totalCarot);
        powerUpSpawner.Init();

        currentPlayer = isCurretPlayerIsRed ? playerRed : playerBlue;

        startTime = DateTime.Now;
    }

    void Update()
    {
        if (!isPaused && !isActivatingPower && !isAdvancedSettings)
        {
            timer -= Time.deltaTime;
            if (GameOverChecker() || timer <= 0f)
            {
                //if (playerRed.GetScore() > playerBlue.GetScore()) playerRed.AddBonusPoint(100);
                //if (playerBlue.GetScore() > playerRed.GetScore()) playerBlue.AddBonusPoint(100);
                int bonusPoint = (int)(duration-timer)*2;
                currentPlayer.AddBonusPoint(bonusPoint);

                if (isTraining)
                {
                    if (playerRed.GetScore() > playerBlue.GetScore())
                    {
                        AgentRed.AddReward(1f);
                        AgentBlue.AddReward(-1f);
                    }
                    if (playerRed.GetScore() < playerBlue.GetScore())
                    {
                        AgentRed.AddReward(-1f);
                        AgentBlue.AddReward(1f);
                    }
                    timer = duration;
                    AgentRed.EndEpisode();
                    AgentBlue.EndEpisode();
                    playerRed.ResetPlayer();
                    playerBlue.ResetPlayer();
                    powerUpManager.ResetPowerUps();
                    vegetableSpawner.ResetAllVegetables();
                    powerUpSpawner.ResetPowerUps();
                }
                else
                {
                    // Simpan data ke database
                    SaveGame();
                    SceneManager.LoadScene("GameOver");
                }
            }
        }
        UpdateUI();
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }
    public Player GetPlayerRed()
    {
        return playerRed;
    }
    public Player GetPlayerBlue()
    {
        return playerBlue;
    }

    private void FixedUpdate()
    {
        if (!isTraining)
        {
            Vector2 movement = move_action.action.ReadValue<Vector2>();
            currentPlayer.MovePlayer(movement);
        }
        if (isDisplayUI) cameraFollow.UpdateCamera(currentPlayer.transform);
    }

    bool GameOverChecker()
    {
        return (playerRed.GetTotalCollectedVegetables() + playerBlue.GetTotalCollectedVegetables()) == (totalPotato + totalCarot);
    }
    void UpdateTimerUI()
    {
        timerSlider.value = timer;
        if (timer>=0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string formattedTime = string.Format("{0:00}.{1:00}", minutes, seconds);
            timerText.text = formattedTime;
        } else
        {
            timerText.text = "00:00";
        }
    }

    void UpdateUI()
    {
        if (isDisplayUI)
        {
            UpdateTimerUI();
            powerUpUI.UpdatePowerUpButton(currentPlayer.GetPowerUpList());
            playerUI.UpdateScore(playerRed.GetScore(), playerBlue.GetScore());
        }
        playerUI.UpdateUI(true, playerRed.GetVegetableType(), playerRed.GetPlayerCaring());
        playerUI.UpdateUI(false, playerBlue.GetVegetableType(), playerBlue.GetPlayerCaring());
    }

    public void ChangePlayer()
    {
        isCurretPlayerIsRed = !isCurretPlayerIsRed;
        currentPlayer = isCurretPlayerIsRed ? playerRed : playerBlue;
    }

    public void SaveGame()
    {

        endTime = DateTime.Now;
        TimeSpan duration = endTime - startTime;
        int durationInSeconds = (int)duration.TotalSeconds;
        GameData gameData = new GameData();
        string savedUserId = PlayerPrefs.GetString("UserId", "");

        gameData.user_id = savedUserId;
        gameData.score = playerBlue.GetScore();
        gameData.time_play = startTime;
        gameData.duration = durationInSeconds;
        gameData.emotion_history = emotionList;
        string json = JsonUtility.ToJson(gameData);
        Debug.Log(json);

        StartCoroutine(PostRequest(gameData));
        PlayerPrefs.SetString("RedScore", playerRed.GetScore().ToString());
        PlayerPrefs.SetString("BlueScore", playerBlue.GetScore().ToString());
        PlayerPrefs.SetString("LastScore", currentPlayer.GetScore().ToString());
    }

    IEnumerator PostRequest(GameData gameData)
    {
        Debug.Log("Start kirim");
        // Konversi GameData menjadi JSON
        string jsonData = JsonUtility.ToJson(gameData);

        // Kirim request POST
        string url = "https://lizard-alive-suitably.ngrok-free.app/history/save_game";
        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        // Cek jika terjadi error saat mengirim request
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Cetak response
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    public void OnPauseButtonClick()
    {
        SetPause(true);
    }
    
    public void OnContinueButtonClick()
    {
        SetPause(false);
    }
    

    public void OnOkButtonClick()
    {

        SetActivatePower(false);
        
    }
    
    public void OnActivatePowerButtonClick(int buttonIndex)
    {
        int currentPowerUpType = currentPlayer.CheckPowerupType(buttonIndex); // 0 = None, 1 = angry, 2 = sad, 3 = fear, 4 = joy
        if (currentPowerUpType > 0)
        {
            string word = GetRandomWord();
            SetActivatePower(true);
            powerUpManager.StartActivatePowerUp(isCurretPlayerIsRed, word, currentPowerUpType);
        }
    }
    
    public float GetRandomEmotionScore()
    {
        return UnityEngine.Random.Range(0, 100);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void OnAdvancedSettingsButtonClick()
    {
        SetAdvancedSettings(true);
    }

    public void OnQuitButtonClick()
    {
        SaveGame();
        SetPause(false);
        SceneManager.LoadScene("GameOver");
    }

    void SetPause(bool isPaused)
    {
        this.isPaused = isPaused;
        pausePopup.SetActive(isPaused);
        //Time.timeScale = isPaused ? 0f : 1f;
    }

    void SetActivatePower(bool isActivatePowerUp)
    {
        bool isPaused = isActivatePowerUp;
        this.isPaused = isPaused;
        //Time.timeScale = isPaused ? 0f : 1f;
        activatePowerUp.CloseResults();
        activatePowerPopup.SetActive(isActivatePowerUp);
        powerUpManager.predictedEmotion = "";
    }
    
    void SetAdvancedSettings(bool isAdvancedSettings)
    {
        bool isPaused = isAdvancedSettings;
        this.isPaused = isPaused;
        //Time.timeScale = isPaused ? 0f : 1f;
        advancedSettingsPopup.SetActive(isAdvancedSettings);
    }
}


[System.Serializable]
public class GameData
{
    public string user_id;
    public string history_id;
    public int score;
    public DateTime time_play;
    public int duration; 
    public List<EmotionHistory> emotion_history = new List<EmotionHistory>();
}

[System.Serializable]
public class EmotionHistory
{
    public string word;
    public string emotion_target;
    public string voice_emotion;
    public float percentage;
    public DateTime time_stamp;
}