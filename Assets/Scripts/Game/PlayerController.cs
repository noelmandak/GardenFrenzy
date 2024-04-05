using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.ScrollRect;

public class PlayerController : MonoBehaviour
{
    public bool isRed = true;
    public float playerSpeed = 250.0f;
    public Rigidbody2D playerRed;
    public Rigidbody2D playerBlue;
    private Rigidbody2D player;
    public int maxCapacity = 5;
    public PowerUpManager PowerUpManager;

    private int playerRedType = 0;
    private int playerRedCaring = 0; //0 is none, 1 is kentang, 2 is wortel
    private int playerRedKentangCount = 0;
    private int playerRedWortelCount = 0;
    private int playerRedScore = 0; 
    private int[] playerRedPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
    public float playerRedSpeed = 250.0f;



    private int playerBlueType = 0;
    private int playerBlueCaring = 0;
    private int playerBlueKentangCount = 0;
    private int playerBlueWortelCount = 0;
    private int playerBlueScore = 0;
    private int[] playerBluePowerUp = new int[] { 0, 0, 0 };
    public float playerBlueSpeed = 250.0f;

    private PlayerUI playerUI;
    public PowerUpUI powerUpUI;

    [SerializeField]
    private InputActionReference move_action;

    public CameraFollow cameraFollow;

    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    void Update()
    {
        Vector2 movement = move_action.action.ReadValue<Vector2>();
        MoveObject(movement);
    }
    private void MoveObject(Vector2 movement)
    {
        if (isRed)
        {
            playerRed.velocity = playerRedSpeed * Time.deltaTime * movement;
            cameraFollow.UpdateCamera(playerRed.transform);
        } else
        {
            playerBlue.velocity = playerBlueSpeed * Time.deltaTime * movement;
            cameraFollow.UpdateCamera(playerBlue.transform);
        }
    }

    public void SetPlayerSpeed(bool isPlayerRed, float speed)
    {
        if (isPlayerRed) playerRedSpeed = speed;
        else playerBlueSpeed = speed;
    }
    public void SetPlayerSpeed(float speed)
    {
        if (isRed) playerRedSpeed = speed;
        else playerBlueSpeed = speed;
    }

    public float GetPlayerSpeed(bool isPlayerRed)
    {
        if (isPlayerRed) return playerRedSpeed;
        return playerBlueSpeed;
    }
    public float GetPlayerSpeed()
    {
        if (isRed) return playerRedSpeed;
        return playerBlueSpeed;
    }

    public void ChangePlayer()
    {
        isRed = !isRed;
        UpdatePowerUpButtonCurrentPlayer();
    }

    public bool CollectPowerUp(bool isPlayerRed, int powerUpType)
    {
        if (isPlayerRed)
        {
            if (playerRedPowerUp[0]!=0 && playerRedPowerUp[1] != 0 && playerRedPowerUp[2] != 0) return false;
            for (int i = 0; i < 3; i++)
            {
                if (playerRedPowerUp[i] != 0) continue;
                playerRedPowerUp[i] = powerUpType;
                if (isRed && isPlayerRed)
                {
                    powerUpUI.UpdatePowerUpButton(playerRedPowerUp);
                } 
                return true;
            }
        } else
        {
            if (playerBluePowerUp[0]!=0 && playerBluePowerUp[1] != 0 && playerBluePowerUp[2] != 0) return false;
            for (int i = 0; i < 3; i++)
            {
                if (playerBluePowerUp[i] != 0) continue;
                playerBluePowerUp[i] = powerUpType;
                if (!isRed && !isPlayerRed)
                {
                    powerUpUI.UpdatePowerUpButton(playerBluePowerUp);
                }
                return true;
            }
        }
        return false;
    }

    public void UpdatePowerUpButtonCurrentPlayer()
    {
        if (isRed) powerUpUI.UpdatePowerUpButton(playerRedPowerUp);
        else powerUpUI.UpdatePowerUpButton(playerBluePowerUp);
    }
    public bool CollectVegetable(bool isPlayerRed, int vegetableType)
    {
        if (isPlayerRed)
        {
            if (playerRedType == 0)
            {
                playerRedType = vegetableType;
            }
            if (vegetableType == playerRedType)
            {
                if (playerRedCaring<maxCapacity)
                {
                    playerRedCaring++;
                    playerUI.UpdateUI(isPlayerRed, vegetableType, playerRedCaring);
                    return true;
                }
            }
        } else
        {
            if (playerBlueType == 0)
            {
                playerBlueType = vegetableType;
            }
            if (vegetableType == playerBlueType)
            {
                if (playerBlueCaring < maxCapacity)
                {
                    playerBlueCaring++;
                    playerUI.UpdateUI(isPlayerRed, vegetableType, playerBlueCaring);
                    return true;
                }
            }

        }
        return false;
    }

    public bool MoveToBox(bool isPlayerRed, int boxType, int point)
    {
        if (isPlayerRed)
        {
            if (boxType == playerRedType)
            {
                if (boxType == 1)
                {
                    playerRedKentangCount += playerRedCaring;
                    playerRedScore += point * playerRedCaring;
                    playerRedCaring = 0;
                    playerRedType = 0;
                    playerUI.UpdateUI(isPlayerRed, 0, playerRedCaring);
                    playerUI.UpdateScore(playerRedScore, playerBlueScore);
                    SaveScore();
                    return true;
                }
                if (boxType == 2)
                {
                    playerRedWortelCount += playerRedCaring;
                    playerRedScore += point * playerRedCaring;
                    playerRedCaring = 0;
                    playerRedType = 0;
                    playerUI.UpdateUI(isPlayerRed, 0, playerRedCaring);
                    playerUI.UpdateScore(playerRedScore, playerBlueScore);
                    SaveScore();
                    return true;
                }
            }
        } else
        {
            if (boxType == playerBlueType)
            {
                if (boxType == 1)
                {
                    playerBlueKentangCount += playerBlueCaring;
                    playerBlueScore += point * playerBlueCaring;
                    playerBlueCaring = 0;
                    playerBlueType = 0;
                    playerUI.UpdateUI(isPlayerRed, 0, playerBlueCaring);
                    playerUI.UpdateScore(playerRedScore, playerBlueScore);
                    SaveScore();
                    return true;
                }
                if (boxType == 2)
                {
                    playerBlueWortelCount += playerBlueCaring;
                    playerBlueScore += point * playerBlueCaring;
                    playerBlueCaring = 0;
                    playerBlueType = 0;
                    playerUI.UpdateUI(isPlayerRed, 0, playerBlueCaring);
                    playerUI.UpdateScore(playerRedScore, playerBlueScore);
                    SaveScore();
                    return true;
                }
            }
        }
        return false;
    }
    public int GetScore(bool isRed)
    {
        if (isRed) return playerRedScore;
        else return playerBlueScore;
    }
    public void SaveScore()
    {
        PlayerPrefs.SetString("LastScore", GetScore(isRed).ToString());
    }

    public int ActivatePower(int index)
    {
        if (isRed)
        {
            int powerUpType = playerRedPowerUp[index];
            if (powerUpType > 0) playerRedPowerUp[index] = 0;
            powerUpUI.UpdatePowerUpButton(playerRedPowerUp);
            return powerUpType;
        }
        else
        {
            int powerUpType = playerBluePowerUp[index];
            if (powerUpType > 0) playerBluePowerUp[index] = 0;
            powerUpUI.UpdatePowerUpButton(playerBluePowerUp);
            return powerUpType;
        }
    }
}
