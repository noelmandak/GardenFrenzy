using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;
using System.Text;
using System;
using System.Net;
using UnityEngine.Networking;
using UnityEditor.PackageManager.Requests;

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
        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        string savedUsername = PlayerPrefs.GetString("Username", "");
        if (!string.IsNullOrEmpty(savedUsername))
        {
            Debug.Log("Player telah menyimpan username: " + savedUsername);
            SceneManager.LoadScene("MainMenu");
        }
        //errorText.gameObject.SetActive(false);
        //StartCoroutine(GetRequest("http://10.252.226.147:5000/"));

    }

    public void OnLoginButtonClick()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(LoginRequest("http://10.252.226.147:5000/user/login", email, password));
        }
        else
        {
            DisplayErrorMessage("Email or Password cannot be empty!");
        }
    }

    IEnumerator GetRequest(string uri)
    {
        Debug.Log("Gett");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    IEnumerator LoginRequest(string uri, string email, string password)
    {
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

            PlayerPrefs.SetString("Username", email);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("Received: " + webRequest.downloadHandler.text);

            var jsonResponse = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);

            PlayerPrefs.SetString("UserId", jsonResponse.user_id);
            PlayerPrefs.SetString("Username", jsonResponse.username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }
    IEnumerator SignInRequest(string uri,string username, string email, string password)
    {
        // Buat JSON string
        string jsonString = "{\"username\":\""+ username + "\",\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
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
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            Debug.Log("Received: " + webRequest.downloadHandler.text);

            var jsonResponse = JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);

            PlayerPrefs.SetString("UserId", jsonResponse.user_id);
            PlayerPrefs.SetString("Username", jsonResponse.username);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }

    void DisplayErrorMessage(string message)
    {
        errorText.text = message;
    }

    public void OnSigninButtonClick()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(SignInRequest("http://10.252.226.147:5000/user/register", username, email, password));
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


}

[Serializable]
public class LoginResponse
{
    public string message;
    public string user_id;
    public string username;
}
