using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool isRed;
    private bool isFacingRight = true;
    private float initialPlayerSpeed = 0;
    private Vector3 initialPosition;
    private float playerSpeed = 0;
    private int maxCapacity = 0 ;
    private int vegetableType = 0;  //0 = none, 1 = potato, 2 = carrot
    private int playerCaring = 0;
    private int potatoCount = 0;
    private int carrotCount = 0;
    private int playerScore = 0;
    private int[] playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
    private bool isDoublePointActive = false;
    private bool isInField = false;
    private bool isInDirtPath = false;

    public GameObject potatoBox;
    public GameObject carrotBox;

    [SerializeField]
    private GameObject FearField;

    private RLAgent agent;
    private GameManager gameManager;
    private Animator animator;

    private void Start()
    {
        initialPosition = gameObject.transform.localPosition;
        agent = gameObject.GetComponent<RLAgent>();
        animator = GetComponent<Animator>();
        gameManager = GetComponentInParent<GameManager>();
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
        vegetableType = 0;  //0 = none, 1 = potato, 2 = carrot
        playerCaring = 0;
        potatoCount = 0;
        carrotCount = 0;
        playerScore = 0;
        //maxCapacity = Random.Range(1, 6);
        maxCapacity = 5;
        Debug.Log($"Capacity = {maxCapacity}");
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
        return potatoCount + carrotCount;
    }

    public void MovePlayer(Vector2 movement)
    {
        if (gameManager.isPaused)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            return;
        }
        float tileSpeed = (isInField ? -1.5f : 0) + (isInDirtPath ? 1.5f : 0);
        gameObject.GetComponent<Rigidbody2D>().velocity = movement * (playerSpeed + tileSpeed);

        animator.SetFloat("Velocity", Mathf.Abs(movement.x) + Mathf.Abs(movement.x));

        if (movement.x > 0) isFacingRight = true;
        else if (movement.x < 0) isFacingRight = false;
        Flip();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Field")) isInField = true;
        if (other.CompareTag("DirtPath")) isInDirtPath = true;
        if (other.CompareTag("Obstacle"))
            agent.AddReward(gameManager.GetEnvParam("reward_colide_with_obstacle", 0));
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Field")) isInField = false;
        if (other.CompareTag("DirtPath")) isInDirtPath = false;
    }

    public bool CollectPowerUp(int powerUpType)
    {
        if (playerPowerUp[0] != 0 && playerPowerUp[1] != 0 && playerPowerUp[2] != 0) return false;
        for (int i = 0; i < 3; i++)
        {
            if (playerPowerUp[i] != 0) continue;
            playerPowerUp[i] = powerUpType;
            //switch(powerUpType)
            //{
            //    case 1: AudioManager.Instance.PlaySFX("pickuppowerup1"); break;
            //    case 2: AudioManager.Instance.PlaySFX("pickuppowerup2"); break;
            //    case 3: AudioManager.Instance.PlaySFX("pickuppowerup3"); break;
            //    case 4: AudioManager.Instance.PlaySFX("pickuppowerup4"); break;
            //}
            agent.AddReward(0.001f);
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
                //if (vegetableType == 1) AudioManager.Instance.PlaySFX("pickuppotato");
                //if (vegetableType == 2) AudioManager.Instance.PlaySFX("pickupcarrot");
                this.playerCaring++;
                agent.AddReward(gameManager.GetEnvParam("reward_collect_a_vegetable", 0.001f));
                return true;
            }
            //if (gameManager.gamePlayCounter>100000) agent.AddReward(-0.0001f);
        }
        agent.AddReward(gameManager.GetEnvParam("reward_collect_wrong_vegetable", -0.001f));

        //if (gameManager.gamePlayCounter > 100000)
        //{
        //    agent.AddReward(-0.0001f);
        //    gameManager.resetGame();
        //}
        return false;
    }

    public bool MoveToBox(int boxType, int point)
    {
        int score = point * playerCaring * (isDoublePointActive ? 2 : 1);
        if (boxType == vegetableType)
        {
            if (boxType == 1) // Kotak adalah kentang
            {
                potatoCount += playerCaring; 
                //AudioManager.Instance.PlaySFX("droppotato");
            }
            if (boxType == 2) // Kotak adalah wortel
            {
                carrotCount += playerCaring;
                //AudioManager.Instance.PlaySFX("dropcarrot");
            }
            agent.AddReward(gameManager.GetEnvParam("reward_put_a_vegetable_in_right_box", 0.001f) * playerCaring + gameManager.GetTimeBonus()*0.1f);
            playerScore += score;
            playerCaring = 0;
            vegetableType = 0;

            return true;

        }
        if (vegetableType != 0)
        {
            agent.AddReward(-0.000001f * score);
            agent.AddReward(gameManager.GetEnvParam("reward_put_a_vegetable_in_wrong_box", -0.001f) * playerCaring + gameManager.GetTimeBonus() * 0.1f);
            //if (gameManager.gamePlayCounter > 100000) agent.AddReward(-0.001f);
        }

        //if (gameManager.gamePlayCounter > 100000) { 
        //    agent.AddReward(-0.001f); 
        //    gameManager.resetGame();
        //}
        return false;
    }

    public int CheckPowerupType(int index)
    {
        int powerUpType = playerPowerUp[index];
        if (powerUpType > 0) playerPowerUp[index] = 0;
        else agent.AddReward(-0.00000001f);
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
        Vector3 playerPowerup = new(playerPowerUp[0] / 4, playerPowerUp[1] / 4, playerPowerUp[2] / 4);
        Vector3 dirToPotatoToBox = (potatoBox.transform.localPosition - transform.localPosition).normalized;
        Vector3 dirToCarrotToBox = (carrotBox.transform.localPosition - transform.localPosition).normalized;
        return new PlayerProperties(isRed, isDoublePointActive, FearField.activeSelf, playerSpeed, maxCapacity, playerCaring, vegetableType, potatoCount, carrotCount, playerScore, playerPowerup, dirToPotatoToBox, dirToCarrotToBox);
    }
    public void RandomizePosition()
    {
        Vector2 randomPositionPlayer = GetEmptyRandomPosition(-10, -10, 10, 10);
        transform.localPosition = new Vector3(randomPositionPlayer.x, randomPositionPlayer.y, transform.localPosition.z);

        Vector2 randomPositionPotatoBox = GetEmptyRandomPosition(-10, -10, 10, 10);
        Vector3 potatoBoxPosition = new Vector3(randomPositionPotatoBox.x, randomPositionPotatoBox.x, potatoBox.transform.localPosition.z);
        potatoBox.transform.localPosition = potatoBoxPosition;

        Vector2 randomPositionCarrotBox = GetEmptyRandomPosition(-10, -10, 10, 10);
        Vector3 carrotBoxPosition = new Vector3(randomPositionCarrotBox.x, randomPositionCarrotBox.y, carrotBox.transform.localPosition.z);
        carrotBox.transform.localPosition = carrotBoxPosition;

    }

    Vector2 GetEmptyRandomPosition(float x1, float y1, float x2, float y2)
    {
        float randomX;
        float randomY;
        bool validPosition;
        do
        {
            randomX = Random.Range(x1, x2);
            randomY = Random.Range(y1, y2);
            validPosition = CheckPosition(randomX, randomY);
        } while (!validPosition);
        return new Vector2(randomX, randomY);
    }

    bool CheckPosition(float x, float y)
    {
        Vector2 spawnPosition = new Vector2(x, y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 3f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle")) return false;
        }
        return true;
    }

    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        if ((isFacingRight && (theScale.x < 0)) || !isFacingRight && (theScale.x > 0))
        {
            transform.localScale = new Vector3(-theScale.x, theScale.y, theScale.z);
            foreach (Transform child in transform)
            {
                Vector3 childScale = child.localScale;
                child.localScale = new Vector3(-childScale.x, childScale.y, childScale.z);
            }
        }
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
    public int CarrotCount { get; private set; }
    public int PlayerScore { get; private set; }
    public Vector3 PlayerPowerUp { get; private set; }
    public Vector3 DirToPotatoBox { get; private set; }
    public Vector3 DirToCarrotBox { get; private set; }

    public PlayerProperties(bool isRed, bool isDoublePointActive, bool isFearFieldActive, float speed, 
        int capacity, int playerCaring, int vegetableType, int potatoCount, int carrotCount, int playerScore, 
        Vector3 playerPowerUp, Vector3 dirToPotatoBox, Vector3 dirToCarrotBox)
    {
        IsRed = isRed;
        IsDoublePointActive = isDoublePointActive;
        IsFearFieldActive = isFearFieldActive;
        PlayerSpeed = speed;
        MaxCapacity = capacity;
        PlayerCaring = playerCaring;
        VegetableType = vegetableType;
        PotatoCount = potatoCount;
        CarrotCount = carrotCount;
        PlayerScore = playerScore;
        PlayerPowerUp = playerPowerUp;
        DirToPotatoBox = dirToPotatoBox;
        DirToCarrotBox = dirToCarrotBox;
    }
}