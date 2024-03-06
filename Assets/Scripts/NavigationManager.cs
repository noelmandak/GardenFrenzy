using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Back");
        SceneManager.LoadScene("MainMenu");
    }
}
