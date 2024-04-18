using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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
        initialPosition = gameObject.transform.localPosition;
    }

    public void Init(float speed, int capacity)
    {
        initialPlayerSpeed = speed;
        playerSpeed = speed;
        maxCapacity = capacity;
    }


    public void ResetPlayer()
    {
        playerSpeed = initialPlayerSpeed;
        vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
        playerCaring = 0;
        potatoCount = 0;
        carotCount = 0;
        playerScore = 0;
        playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
        isDoublePointActive = false;
        gameObject.transform.localPosition = initialPosition;
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

    public void ResetSpeed()
    {
        playerSpeed = initialPlayerSpeed;
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
            int score = point * playerCaring * (isDoublePointActive ? 2 : 1);
            playerScore += score;
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

    public PlayerProperties GetPlayerProperties()
    {
        Vector3 playerPowerup = new(playerPowerUp[0], playerPowerUp[1], playerPowerUp[2]);
        Vector3 dirToPotatoToBox = (potatoBox.transform.localPosition - transform.localPosition).normalized;
        Vector3 dirToCarotToBox = (carotBox.transform.localPosition - transform.localPosition).normalized;
        return new PlayerProperties(isRed, isDoublePointActive, FearField.activeSelf, playerSpeed, maxCapacity, playerCaring, vegetableType, potatoCount, carotCount, playerScore, playerPowerup, dirToPotatoToBox, dirToCarotToBox);
    }
}


public class PlayerProperties
{
    public bool IsRed { get; private set; }
    public bool IsDoublePointActive { get; private set; }
    public bool IsFearFieldActive { get; private set; }
    public float PlayerSpeed { get; private set; }
    public int MaxCapacity { get; private set; }
    public int PlayerCaring { get; private set; }
    public int VegetableType { get; private set; }
    public int PotatoCount { get; private set; }
    public int CarotCount { get; private set; }
    public int PlayerScore { get; private set; }
    public Vector3 PlayerPowerUp { get; private set; }
    public Vector3 DirToPotatoBox { get; private set; }
    public Vector3 DirToCarotBox { get; private set; }

    public PlayerProperties(bool isRed, bool isDoublePointActive, bool isFearFieldActive, float speed, int capacity, int playerCaring, int vegetableType, int potatoCount, int carotCount, int playerScore, Vector3 playerPowerUp, Vector3 dirToPotatoBox, Vector3 dirToCarotBox)
    {
        IsRed = isRed;
        IsDoublePointActive = isDoublePointActive;
        IsFearFieldActive = isFearFieldActive;
        PlayerSpeed = speed;
        MaxCapacity = capacity;
        PlayerCaring = playerCaring;
        VegetableType = vegetableType;
        PotatoCount = potatoCount;
        CarotCount = carotCount;
        PlayerScore = playerScore;
        PlayerPowerUp = playerPowerUp;
        DirToPotatoBox = dirToPotatoBox;
        DirToCarotBox = dirToCarotBox;
    }
}