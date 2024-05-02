using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Back");
        AudioManager.Instance.PlaySFX("buttonpress1");
        SceneManager.LoadScene("MainMenu");
    }

    public void OnLogoutButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        // Menghapus username yang tersimpan
        PlayerPrefs.DeleteKey("Username");

        // Pindah ke scene LoginSignup
        SceneManager.LoadScene("LoginSignup");
    }
    // Fungsi untuk tombol Play
    public void OnPlayButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        // Pindah ke scene Game
        SceneManager.LoadScene("Level1");
    }

    // Fungsi untuk tombol Player History
    public void OnLeaderboardButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        // Pindah ke scene Player History
        SceneManager.LoadScene("Leaderboard");
    }
    // Fungsi untuk tombol Player History
    public void OnPlayerHistoryButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        // Pindah ke scene Player History
        SceneManager.LoadScene("PlayerHistory");
    }

    // Fungsi untuk tombol Settings
    public void OnSettingsButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        // Pindah ke scene Settings
        SceneManager.LoadScene("Settings");
    }

    // Fungsi untuk tombol Exit
    public void OnExitButtonClick()
    {
        // Keluar dari aplikasi (hanya berfungsi di build)
        AudioManager.Instance.PlaySFX("buttonpress1");
        Application.Quit();
    }
}
