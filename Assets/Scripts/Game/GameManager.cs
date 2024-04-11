using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject pausePopup;
    public GameObject activatePowerPopup;
    public GameObject advancedSettingsPopup;
    public ActivatePowerUp activatePowerUp;
    public GameObject gameCanvas;
    public Slider timerSlider;

    private float playerSpeed = 5;
    private int maxCapacity = 2;
    private int totalPotato = 1;
    private int totalCarot = 1;
    private float duration = 30f;
    private float timer;
    private bool isPaused = false;
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
        playerRed.RandomizePosition();

        currentPlayer = isCurretPlayerIsRed ? playerRed : playerBlue;
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
                //playerRed.AddBonusPoint(bonusPoint);

                if (isTraining)
                {

                    //float timerPenalty = ((duration-timer)/duration) * -0.001f; // Give the agent a penalty if it becomes stuck somewhere.
                    //AgentRed.AddReward(timerPenalty);

                    if (GameOverChecker()) AgentRed.AddReward(1f); // Give the agent a reward if it finished the level.

                    //if (playerRed.GetScore() > playerBlue.GetScore())
                    //{
                    //    AgentRed.AddReward(1f);
                    //    AgentBlue.AddReward(-1f);
                    //}
                    //if (playerRed.GetScore() < playerBlue.GetScore())
                    //{
                    //    AgentRed.AddReward(-1f);
                    //    AgentBlue.AddReward(1f);
                    //}
                    timer = duration;
                    AgentRed.EndEpisode();
                    AgentBlue.EndEpisode();
                    playerBlue.ResetPlayer();
                    powerUpManager.ResetPowerUps();
                    vegetableSpawner.ResetAllVegetables();
                    powerUpSpawner.ResetPowerUps();
                    playerRed.RandomizePosition();
                    playerRed.ResetPlayer();
                }
                else
                {
                    SaveScore();
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
        if (isDisplayUI && cameraFollow) cameraFollow.UpdateCamera(currentPlayer.transform);
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

    public void SaveScore()
    {
        PlayerPrefs.SetString("RedScore", playerRed.GetScore().ToString());
        PlayerPrefs.SetString("BlueScore", playerBlue.GetScore().ToString());
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
        SetAdvancedSettings(false);
    }
    
    public void OnActivatePowerButtonClick(int buttonIndex)
    {
        int currentPowerUpType = currentPlayer.ActivatePower(buttonIndex);
        if (currentPowerUpType > 0)
        {
            int star = GetStar();
            activatePowerUp.currentPowerUpType = currentPowerUpType;
            activatePowerUp.SetCommand();
            activatePowerUp.SetStar(star);
            powerUpManager.ActivatePower(isCurretPlayerIsRed, currentPowerUpType, star);
            SetActivatePower(true);

        }
    }
    
    public int GetStar()
    {
        return Random.Range(1, 4);
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
        SaveScore();
        SetPause(false);
        SceneManager.LoadScene("GameOver");
    }

    void SetPause(bool isPaused)
    {
        this.isPaused = isPaused;
        pausePopup.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void SetActivatePower(bool isActivatePowerUp)
    {
        bool isPaused = isActivatePowerUp;
        this.isPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        activatePowerPopup.SetActive(isActivatePowerUp);
    }
    
    void SetAdvancedSettings(bool isAdvancedSettings)
    {
        bool isPaused = isAdvancedSettings;
        this.isPaused = isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        advancedSettingsPopup.SetActive(isAdvancedSettings);
    }
}
