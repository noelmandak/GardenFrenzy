using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool isRed;

    private float playerSpeed = 0;
    private int maxCapacity = 0 ;
    private int vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
    private int playerCaring = 0;
    private int potatoCount = 0;
    private int carotCount = 0;
    private int playerScore = 0;
    private int[] playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
    private bool isDoublePointActive = false;

    [SerializeField]
    private GameObject FearField;

    public void Init(float speed, int capacity)
    {
        playerSpeed = speed;
        maxCapacity = capacity;
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
            playerScore += point * playerCaring * (isDoublePointActive ? 2 : 1);
            playerCaring = 0;
            vegetableType = 0;
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
