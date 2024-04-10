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
    private int maxCapacity = 5;
    private int totalPotato = 15;
    private int totalCarot = 10;
    private float duration = 100f;
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


    [SerializeField]
    private InputActionReference move_action;
    public Player playerRed;
    public Player playerBlue;
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

        currentPlayer = isCurretPlayerIsRed ? playerRed : playerBlue;
        Debug.Log("Started");
    }

    void Update()
    {
        Debug.Log("Updated");

        if (!isPaused && !isActivatingPower && !isAdvancedSettings)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                SceneManager.LoadScene("GameOver");
            }
            if (GameOverChecker())
            { 
                int bonusPoint = (int)(duration-timer)*2;
                currentPlayer.AddBonusPoint(bonusPoint);
                SceneManager.LoadScene("GameOver");
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
        Vector2 movement = move_action.action.ReadValue<Vector2>();
        currentPlayer.MovePlayer(movement);
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

    public void SaveScore()
    {
        PlayerPrefs.SetString("LastScore", currentPlayer.GetScore().ToString());
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
        SceneManager.LoadScene("GameOver");
        SetPause(false);
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
