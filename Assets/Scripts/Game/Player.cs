using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Player : Agent
{
    [SerializeField]
    private bool isRed;

    private float initialPlayerSpeed = 0;
    private Vector3 initialPosition;
    private float playerSpeed = 0;
    private int maxCapacity = 0 ;
    private int vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
    private int playerCaring = 0;
    private int potatoCount = 0;
    private int carotCount = 0;
    private int playerScore = 0;
    private int[] playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
    private bool isDoublePointActive = false;

    public GameObject potatoBox;
    public GameObject carotBox;

    [SerializeField]
    private GameObject FearField;

    private void Start()
    {
        initialPosition = gameObject.transform.position;
    }

    public void Init(float speed, int capacity)
    {
        initialPlayerSpeed = speed;
        playerSpeed = speed;
        maxCapacity = capacity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(!isRed); 
        sensor.AddObservation(vegetableType);
        sensor.AddObservation(playerCaring);
        sensor.AddObservation(potatoCount);
        sensor.AddObservation(carotCount);
        sensor.AddObservation(playerScore);
        sensor.AddObservation(new Vector3(playerPowerUp[0], playerPowerUp[1], playerPowerUp[2]));

        Vector3 dirToPotatoToBox = (potatoBox.transform.position - transform.position).normalized;
        Vector3 dirToCarotToBox = (carotBox.transform.position - transform.position).normalized;
        sensor.AddObservation(new Vector2(dirToPotatoToBox.x, dirToPotatoToBox.y));
        sensor.AddObservation(new Vector2(dirToCarotToBox.x, dirToCarotToBox.y));
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);
        //Debug.Log(actions.ContinuousActions[1]);
        var x = actions.ContinuousActions[0];
        var y = actions.ContinuousActions[1];
        MovePlayer(new Vector2(x, y));

        if (actions.DiscreteActions[0]>0) ActivatePower(0);
        if (actions.DiscreteActions[1]>0) ActivatePower(1);
        if (actions.DiscreteActions[2]>0) ActivatePower(2);

    }

    public void ResetPlayer()
    {
        playerSpeed = initialPlayerSpeed;
        maxCapacity = 0;
        vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
        playerCaring = 0;
        potatoCount = 0;
        carotCount = 0;
        playerScore = 0;
        playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
        isDoublePointActive = false;
        gameObject.transform.position = initialPosition;
        Debug.Log("player reseted");
    }
    public void GameOver()
    {
        EndEpisode();
        ResetPlayer();
    }
    public float PlayerSpeed
    {
        get { return playerSpeed; }
        set { playerSpeed = value; }
    }

    public int GetScore()
    {
        return playerScore;
    }

    public int GetVegetableType()
    {
        return vegetableType;
    }
    public int GetPlayerCaring()
    {
        return playerCaring;
    }

    public void AddBonusPoint(int bonusPoint)
    {
        playerScore += bonusPoint;
    }

    public int GetTotalCollectedVegetables()
    {
        return potatoCount + carotCount;
    }

    public void MovePlayer(Vector2 movement)
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = movement * playerSpeed;
    }

    public bool CollectPowerUp(int powerUpType)
    {
        if (playerPowerUp[0] != 0 && playerPowerUp[1] != 0 && playerPowerUp[2] != 0) return false;
        for (int i = 0; i < 3; i++)
        {
            if (playerPowerUp[i] != 0) continue;
            playerPowerUp[i] = powerUpType;
            AddReward(0.05f);
            return true;
        }
        return false;
    }

    public bool CollectVegetable(int vegetableType)
    {
        if (this.vegetableType == 0)
        {
            this.vegetableType = vegetableType;
        }
        if (vegetableType == this.vegetableType)
        {
            if (this.playerCaring < maxCapacity)
            {
                this.playerCaring++;
                AddReward(0.01f);
                return true;
            }
        }
        return false;
    }

    public bool MoveToBox(int boxType, int point)
    {
        if (boxType == vegetableType)
        {
            if (boxType == 1) potatoCount += playerCaring; // Kotak adalah kentang
            if (boxType == 2) carotCount += playerCaring;// Kotak adalah wortel
            int score = point * playerCaring * (isDoublePointActive ? 2 : 1);
            playerScore += score;
            playerCaring = 0;
            vegetableType = 0;
            AddReward(score*0.001f);
            return true;

        }
        return false;
    }

    public int ActivatePower(int index)
    {
        int powerUpType = playerPowerUp[index];
        if (powerUpType > 0) playerPowerUp[index] = 0;
        return powerUpType;
    }

    public int[] GetPowerUpList()
    {
        return playerPowerUp;
    }

    public void SetPlayerSpeed(float speedMultiplier)
    {
        playerSpeed *= speedMultiplier;
    }
    public void SetDoublePoints(bool active)
    {
        isDoublePointActive = active;
    }

    public void SetFearField(bool active)
    {
        FearField.SetActive(active);

    }
}
