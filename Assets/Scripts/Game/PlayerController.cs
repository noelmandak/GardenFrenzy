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
    public float playerSpeed = 300.0f;
    public Rigidbody2D playerRed;
    public Rigidbody2D playerBlue;
    private Rigidbody2D player;
    public int maxCapacity = 5;

    private int playerRedType = 0;
    private int playerRedCaring = 0; //0 is none, 1 is kentang, 2 is wortel
    private int playerRedKentangCount = 0;
    private int playerRedWortelCount = 0;
    private int playerRedScore = 0;
    

    private int playerBlueType = 0;
    private int playerBlueCaring = 0;
    private int playerBlueKentangCount = 0;
    private int playerBlueWortelCount = 0;
    private int playerBlueScore = 0;

    private PlayerUI playerUI;

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
        player = isRed ? playerRed : playerBlue;
        player.velocity = playerSpeed * Time.deltaTime * movement;
        cameraFollow.UpdateCamera(player.transform);
    }

    public void SetPlayerSpeed(float speed)
    {
        playerSpeed = speed;
    }

    public float GetPlayerSpeed()
    {
        return playerSpeed;
    }

    public void ChangePlayer()
    {
        isRed = !isRed;
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
}
