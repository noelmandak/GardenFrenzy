using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;

public class LoginSignupManager : MonoBehaviour
{

    public GameObject loginDisplay;
    public GameObject signinDisplay;
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI errorText;
    public TextMeshProUGUI modeText;
    private string mode = "login";

    void Start()
    {
        string savedUsername = PlayerPrefs.GetString("Username", "");
        if (!string.IsNullOrEmpty(savedUsername))
        {
            Debug.Log("Player telah menyimpan username: " + savedUsername);
            SceneManager.LoadScene("MainMenu");
        }
        errorText.gameObject.SetActive(false);
    }

    public void OnLoginButtonClick()
    {

        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            PlayerPrefs.SetString("Username", email);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            DisplayErrorMessage("Email or Password cannot be empty!");
        }
    }
    public void OnSigninButtonClick()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            DisplayErrorMessage("Username, Email, or Password cannot be empty!");
        }
    }
    
    public void OnChangeButtonClick()
    {
      
        if (mode == "login")
        {
            mode = "signin";
            loginDisplay.SetActive(false);
            signinDisplay.SetActive(true);
            modeText.text = "Login";
        }
        else { 
            mode = "login";
            loginDisplay.SetActive(true);
            signinDisplay.SetActive(false);
            modeText.text = "Sign in";

        }
        DisplayErrorMessage("");
    }

    void DisplayErrorMessage(string message)
    {
        errorText.text = message;
        errorText.color = Color.red;
        errorText.gameObject.SetActive(true);
    }
    
}
