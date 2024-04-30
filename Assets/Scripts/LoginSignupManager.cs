using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Net;

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
    string BASE_URL = "https://lizard-alive-suitably.ngrok-free.app/";
    void Start()
    {
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
        AudioManager.Instance.PlaySFX("buttonpress1");
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(LoginRequest(email, password));
        }
        else
        {
            SSTools.ShowMessage("No microphone found.", SSTools.Position.bottom, SSTools.Time.twoSecond);

            //DisplayErrorMessage("Email or Password cannot be empty!");
        }
    }

    //IEnumerator GetRequest(string uri)
    //{
    //    Debug.Log("Gett");
    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
    //    {
    //        yield return webRequest.SendWebRequest();
    //        if (webRequest.isNetworkError)
    //        {
    //            Debug.Log("Error: " + webRequest.error);
    //        }
    //        else
    //        {
    //            Debug.Log(webRequest.downloadHandler.text);
    //        }
    //    }
    //}

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
                DisplayErrorMessage("Incorrect Email or Password!");

            }
            else
            {
                SSTools.ShowMessage("Error: " + webRequest.error, SSTools.Position.bottom, SSTools.Time.twoSecond);
                //PlayerPrefs.SetString("Username", email);
                //PlayerPrefs.SetString("UserId", "0");
                //PlayerPrefs.Save();
                //SceneManager.LoadScene("MainMenu");
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
            SSTools.ShowMessage("Error: " + webRequest.error, SSTools.Position.bottom, SSTools.Time.twoSecond);

            //PlayerPrefs.SetString("Username", username);
            //PlayerPrefs.SetString("UserId", "0");
            //PlayerPrefs.Save();
            //SceneManager.LoadScene("MainMenu");
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

    void DisplayErrorMessage(string message)
    {
        errorText.text = message;
    }

    public void OnSigninButtonClick()
    {
        AudioManager.Instance.PlaySFX("buttonpress1");
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
        {
            StartCoroutine(SignInRequest(username, email, password));
        }
        else
        {
            DisplayErrorMessage("Username, Email, or Password cannot be empty!");
            SSTools.ShowMessage("Username, Email, or Password cannot be empty!", SSTools.Position.bottom, SSTools.Time.twoSecond);

        }
    }
    public void OnChangeButtonClick()
    {
      
        AudioManager.Instance.PlaySFX("buttonpress1");
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
