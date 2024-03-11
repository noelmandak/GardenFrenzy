using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject pausePopup;
    public GameObject activatePowerPopup;

    private float timer = 60f;
    private bool isPaused = false;
    private bool isActivatingPower = false;


    private void Start()
    {
        isPaused = false;
        isActivatingPower = false;
    }

    void Update()
    {
        if (!isPaused && !isActivatingPower)
        {
            // Kurangi waktu seiring berjalannya waktu
            timer -= Time.deltaTime;
            UpdateTimerText();

            // Cek jika waktu habis
            if (timer <= 0f)
            {
                // Pindah ke scene GameOver
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    void UpdateTimerText()
    {
        // Tampilkan waktu dalam format yang diinginkan pada teks
        timerText.text = "Time: " + Mathf.Round(timer).ToString();
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
    }
    public void OnActivatePowerButtonClick()
    {
        // Tampilkan popup pause
        SetActivatePower(true);
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
}
