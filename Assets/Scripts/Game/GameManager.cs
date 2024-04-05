using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject pausePopup;
    public GameObject activatePowerPopup;
    public GameObject advancedSettingsPopup;
    public Slider timerSlider;

    private float duration = 100f;
    private float timer;
    private bool isPaused = false;
    private bool isActivatingPower = false;
    private bool isAdvancedSettings = false;

    public PlayerController playerController;
    public ActivatePowerUp activatePowerUp;
    public PowerUpManager powerUpManager;

    private void Start()
    {
        isPaused = false;
        isActivatingPower = false;
        isAdvancedSettings = false;
        timerSlider.maxValue = duration;
        timer = duration;
    }

    void Update()
    {
        if (!isPaused && !isActivatingPower && !isAdvancedSettings)
        {
            // Kurangi waktu seiring berjalannya waktu
            timer -= Time.deltaTime;
            UpdateTimerText();
            timerSlider.value = timer;

            // Cek jika waktu habis
            if (timer <= 0f)
            {
                // Pindah ke scene GameOver
                SceneManager.LoadScene("GameOver");
            }
            if (playerController.gameOverChecker())
            { 
                int bonusPoint = (int)(duration-timer)*2;
                playerController.claimBonusTimePoint(bonusPoint);
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    void UpdateTimerText()
    {
        if (timer>=0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);

            // Format waktu menjadi "mm:ss"
            string formattedTime = string.Format("{0:00}.{1:00}", minutes, seconds);
            timerText.text = formattedTime;
        } else
        {
            timerText.text = "00:00";
        }
    }

    

    // Fungsi untuk memanggil saat tombol pause ditekan
    public void OnPauseButtonClick()
    {
        // Tampilkan popup pause
        SetPause(true);
    }

    // Fungsi untuk memanggil saat tombol Continue di popup ditekan
    public void OnContinueButtonClick()
    {
        // Sembunyikan popup pause
        SetPause(false);
    }

    // Fungsi untuk memanggil saat tombol Continue di popup ditekan
    public void OnOkButtonClick()
    {
        // Sembunyikan popup pause
        SetActivatePower(false);
        SetAdvancedSettings(false);
    }
    public void OnActivatePowerButtonClick(int buttonIndex)
    {
        // Tampilkan popup pause
        int currentPowerUpType = playerController.ActivatePower(buttonIndex);
        if (currentPowerUpType > 0)
        {
            activatePowerUp.currentPowerUpType = currentPowerUpType;
            //SetActivatePower(true);
            //activatePowerUp.SetCommand();

            Debug.Log($"powerup activated {currentPowerUpType}");
            powerUpManager.ActivatePower(playerController.isRed, currentPowerUpType, GetStar());

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
        // Tampilkan popup pause
        SetAdvancedSettings(true);
    }

    // Fungsi untuk memanggil saat tombol Quit di popup ditekan
    public void OnQuitButtonClick()
    {
        // Pindah ke scene GameOver
        SceneManager.LoadScene("GameOver");
        SetPause(false);
    }

    void SetPause(bool isPaused)
    {
        // Set status pause
        this.isPaused = isPaused;

        // Aktifkan atau nonaktifkan popup pause
        pausePopup.SetActive(isPaused);

        // Time.timeScale digunakan untuk menghentikan atau melanjutkan waktu dalam permainan
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
