using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Net;
using UnityEngine.UI;

public class LoginSignupManager : MonoBehaviour
{
    public GameObject LoginForm, SignInForm;

    public InputField Login_EmailInput, Login_PasswordInput;
    public InputField SignIn_UsernameInput, SignIn_EmailInput, SignIn_PasswordInput;
    public Text ButtonModeText;

    private string mode = "login";
    string BASE_URL = "https://lizard-alive-suitably.ngrok-free.app/";


    void Start()
    {
        string savedUsername = PlayerPrefs.GetString("Username", "");
        if (!string.IsNullOrEmpty(savedUsername))
        {
            Debug.Log("Player telah menyimpan username: " + savedUsername);
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnLoginButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        string email = Login_EmailInput.text;
        string password = Login_PasswordInput.text;

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(LoginRequest(email, password));
        }
        else
        {
            SSTools.ShowMessage("No microphone found.", SSTools.Position.bottom, SSTools.Time.twoSecond);
        }
    }

    IEnumerator LoginRequest(string email, string password)
    {
        string uri = BASE_URL + "user/login";
        // Buat JSON string
        string jsonString = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
        Debug.Log("Posting JSON: " + jsonString);

        // Set header
        UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + webRequest.error);
            if (webRequest.error == "HTTP/1.1 401 Unauthorized")
            {
                SSTools.ShowMessage("Incorrect Email or Password!", SSTools.Position.bottom, SSTools.Time.twoSecond);
            }
            else
            {
                SSTools.ShowMessage("Error: " + webRequest.error, SSTools.Position.bottom, SSTools.Time.twoSecond);
            }
        }
        else
        {
            Debug.Log("Received: " + webRequest.downloadHandler.text);
            SSTools.ShowMessage("Login Success", SSTools.Position.bottom, SSTools.Time.twoSecond);

            var jsonResponse = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);

            PlayerPrefs.SetString("UserId", jsonResponse.user_id);
            PlayerPrefs.SetString("Username", jsonResponse.username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");

        }
    }
    IEnumerator SignInRequest(string username, string email, string password)
    {
        // Buat JSON string
        string uri = BASE_URL + "user/register";
        string jsonString = "{\"username\":\""+ username + "\",\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
        Debug.Log(uri);
        Debug.Log("Posting JSON: " + jsonString);

        // Set header
        UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        // Send request
        yield return webRequest.SendWebRequest();


        if (webRequest.error == "HTTP/1.1 400 Bad Request")
        {
            SSTools.ShowMessage("Username or email already taken!", SSTools.Position.bottom, SSTools.Time.twoSecond);
        }
        else if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
            webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + webRequest.error);
            SSTools.ShowMessage("Error: " + webRequest.error, SSTools.Position.bottom, SSTools.Time.twoSecond);
        }
        else
        {
            SSTools.ShowMessage("Sign In Success", SSTools.Position.bottom, SSTools.Time.twoSecond);
            Debug.Log("Received: " + webRequest.downloadHandler.text);

            var jsonResponse = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);

            PlayerPrefs.SetString("UserId", jsonResponse.user_id);
            PlayerPrefs.SetString("Username", jsonResponse.username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnSigninButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        string username = SignIn_UsernameInput.text;
        string email = SignIn_EmailInput.text;
        string password = SignIn_PasswordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(SignInRequest(username, email, password));
        }
        else
        {
            SSTools.ShowMessage("Username, Email, or Password cannot be empty!", SSTools.Position.bottom, SSTools.Time.twoSecond);

        }
    }
    public void OnChangeButtonClick()
    {
      
        AudioManager.Instance.PlaySFX("buttonpress1");
        if (mode == "login")
        {
            mode = "signin";
            LoginForm.SetActive(false);
            SignInForm.SetActive(true);
            ButtonModeText.text = "Login";
        }
        else { 
            mode = "login";
            LoginForm.SetActive(true);
            SignInForm.SetActive(false);
            ButtonModeText.text = "Sign in";

        }
    }


}

[Serializable]
public class LoginResponse
{
    public string message;
    public string user_id;
    public string username;
}
