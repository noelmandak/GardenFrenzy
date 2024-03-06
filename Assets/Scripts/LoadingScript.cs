using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    IEnumerator Start()
    {
        // Tunggu selama 10 detik
        yield return new WaitForSeconds(3f);

        // Pindah ke scene selanjutnya
        LoadNextScene();
    }
    
    void LoadNextScene()
    {
        // Tentukan logika untuk menentukan scene berikutnya berdasarkan status login atau kondisi lainnya
        // Contoh: Jika player sudah login, pindah ke "MainMenu"; jika belum, pindah ke "LoginSignup"
        string nextScene = DetermineNextScene();

        // Pindahkan ke scene berikutnya
        SceneManager.LoadScene(nextScene);
    }

    string DetermineNextScene()
    {
        // Tentukan logika untuk menentukan scene berikutnya
        // Misalnya, jika player sudah login, kembalikan "MainMenu"; jika belum, kembalikan "LoginSignup"
        // Kamu dapat menggunakan PlayerPrefs, data pengguna, atau metode otentikasi lainnya untuk menentukan status login
        return "LoginSignup";
    }
}
