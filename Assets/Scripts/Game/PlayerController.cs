using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.ScrollRect;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class PlayerController : Agent
{
    public bool isRed = true;
    public float playerSpeed = 300.0f;
    public GameObject playerRedGO;
    public GameObject playerBlueGO;
    public Rigidbody2D playerRed;
    public Rigidbody2D playerBlue;
    private Rigidbody2D player;
    public int maxCapacity = 5;
    public AnotherPlayer anotherPlayer;
    public GameManager gameManager;

    public int playerRedType = 0;
    public int playerRedCaring = 0; //0 is none, 1 is kentang, 2 is wortel
    public int playerRedKentangCount = 0;
    public int playerRedWortelCount = 0;
    public int playerRedScore = 0; 
    public int[] playerRedPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow


    public int playerBlueType = 0;
    public int playerBlueCaring = 0;
    public int playerBlueKentangCount = 0;
    public int playerBlueWortelCount = 0;
    public int playerBlueScore = 0;
    public int[] playerBluePowerUp = new int[] { 0, 0, 0 };

    private PlayerUI playerUI;
    public PowerUpUI powerUpUI;

    [SerializeField]
    private InputActionReference move_action;

    //public CameraFollow cameraFollow;

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(!isRed);
        sensor.AddObservation(playerRedGO.transform.localPosition);
        sensor.AddObservation(playerBlueGO.transform.localPosition);

        sensor.AddObservation(playerRedType);
        sensor.AddObservation(playerRedCaring);
        sensor.AddObservation(playerRedKentangCount);
        sensor.AddObservation(playerRedWortelCount);
        sensor.AddObservation(playerRedScore);
        sensor.AddObservation(new Vector3(playerRedPowerUp[0], playerRedPowerUp[1], playerRedPowerUp[2]));

        sensor.AddObservation(playerBlueType);
        sensor.AddObservation(playerBlueCaring);
        sensor.AddObservation(playerBlueKentangCount);
        sensor.AddObservation(playerBlueWortelCount);
        sensor.AddObservation(playerBlueScore);
        sensor.AddObservation(new Vector3(playerBluePowerUp[0], playerBluePowerUp[1], playerBluePowerUp[2]));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);
        //Debug.Log(actions.ContinuousActions[1]);
        var x = actions.ContinuousActions[0];
        var y = actions.ContinuousActions[1];
        MovePlayer(!isRed, new Vector2(x, y));
    }
    
    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    void Update()
    {
        Vector2 movement = move_action.action.ReadValue<Vector2>();
        MoveObject(movement);
    }


    public void resetPlayer()
    {
        playerRedType = 0;
        playerRedCaring = 0; //0 is none, 1 is kentang, 2 is wortel
        playerRedKentangCount = 0;
        playerRedWortelCount = 0;
        playerRedScore = 0;
        playerRedPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow

        playerBlueType = 0;
        playerBlueCaring = 0;
        playerBlueKentangCount = 0;
        playerBlueWortelCount = 0;
        playerBlueScore = 0;
        playerBluePowerUp = new int[] { 0, 0, 0 };


        playerUI.UpdateUI(true, 0, 0);
        playerUI.UpdateUI(false, 0, 0);

        playerUI.UpdateScore(playerRedScore, playerBlueScore);

        powerUpUI.UpdatePowerUpButton(playerRedPowerUp);
        powerUpUI.UpdatePowerUpButton(playerBluePowerUp);
        playerRedGO.transform.localPosition = new Vector3(-3, 0, 0);
        playerBlueGO.transform.localPosition = new Vector3(3, 0, 0);
        Debug.Log("player reseted");
    }

    private void MoveObject(Vector2 movement)
    {
        player = isRed ? playerRed : playerBlue;
        player.velocity = playerSpeed * Time.deltaTime * movement;
        //cameraFollow.UpdateCamera(player.transform);
    }

    public void MovePlayer(bool isPlayerRed, Vector2 movement)
    {
        player = isPlayerRed ? playerRed : playerBlue;
        player.velocity = playerSpeed * Time.deltaTime * movement;
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
                anotherPlayer.AddRewards(0.005f - gameManager.GetPenalty());
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
                AddReward(0.005f - gameManager.GetPenalty());
                return true;
            }
        }
        return false;
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
                    anotherPlayer.AddRewards(0.003f - gameManager.GetPenalty());
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
                    AddReward(0.003f-gameManager.GetPenalty());
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
                    anotherPlayer.AddRewards((point * playerRedCaring)/300-gameManager.GetPenalty());
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
                    anotherPlayer.AddRewards((point * playerRedCaring) / 300 - gameManager.GetPenalty());
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
                    AddReward((point * playerBlueCaring) / 300 - gameManager.GetPenalty());
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
                    AddReward((point * playerBlueCaring) / 300 - gameManager.GetPenalty());
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
    public void GameOver()
    {
        EndEpisode();
        resetPlayer();
    }
    public void AddRewards(float reward)
    {
        AddReward(reward);
    }
}
