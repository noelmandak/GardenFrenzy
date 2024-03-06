using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    // Start is called before the first frame update
    void Start()
    {
        // Mengambil username yang sudah tersimpan
        string savedUsername = PlayerPrefs.GetString("Username", "");

        // Menampilkan username pada komponen Text
        usernameText.text = "Welcome, " + savedUsername + "!";

    }

}
