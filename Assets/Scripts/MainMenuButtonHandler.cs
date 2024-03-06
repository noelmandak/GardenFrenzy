using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonHandler : MonoBehaviour
{
    public void OnLogoutButtonClick()
    {
        // Menghapus username yang tersimpan
        PlayerPrefs.DeleteKey("Username");

        // Pindah ke scene LoginSignup
        SceneManager.LoadScene("LoginSignup");
    }
    // Fungsi untuk tombol Play
    public void OnPlayButtonClick()
    {
        // Pindah ke scene Game
        SceneManager.LoadScene("GameScene");
    }

    // Fungsi untuk tombol Player History
    public void OnPlayerHistoryButtonClick()
    {
        // Pindah ke scene Player History
        SceneManager.LoadScene("PlayerHistory");
    }

    // Fungsi untuk tombol Settings
    public void OnSettingsButtonClick()
    {
        // Pindah ke scene Settings
        SceneManager.LoadScene("Settings");
    }

    // Fungsi untuk tombol Exit
    public void OnExitButtonClick()
    {
        // Keluar dari aplikasi (hanya berfungsi di build)
        Application.Quit();
    }
}

