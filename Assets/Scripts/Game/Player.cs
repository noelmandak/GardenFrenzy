using System.Collections;
using System.Collections.Generic;
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
    private int vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
    private int playerCaring = 0;
    private int potatoCount = 0;
    private int carotCount = 0;
    private int playerScore = 0;
    private int[] playerPowerUp = new int[] { 0, 0, 0 }; // 1 = red, 2 = blue, 3 = purple, 4 = yellow
    private bool isDoublePointActive = false;
    private bool isInField = false;
    private bool isInDirtPath = false;

    public GameObject potatoBox;
    public GameObject carotBox;

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
        vegetableType = 0;  //0 = none, 1 = potato, 2 = carot
        playerCaring = 0;
        potatoCount = 0;
        carotCount = 0;
        playerScore = 0;
        maxCapacity = Random.Range(1, 6);
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
        return potatoCount + carotCount;
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
            switch(powerUpType)
            {
                case 1: AudioManager.Instance.PlaySFX("pickuppowerup1"); break;
                case 2: AudioManager.Instance.PlaySFX("pickuppowerup2"); break;
                case 3: AudioManager.Instance.PlaySFX("pickuppowerup3"); break;
                case 4: AudioManager.Instance.PlaySFX("pickuppowerup4"); break;
            }
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
                if (vegetableType == 1) AudioManager.Instance.PlaySFX("pickuppotato");
                if (vegetableType == 2) AudioManager.Instance.PlaySFX("pickupcarrot");
                this.playerCaring++;
                agent.AddReward(0.0001f);
                return true;
            }
            //if (gameManager.gamePlayCounter>100000) agent.AddReward(-0.0001f);
        }
        agent.AddReward(-0.00001f);
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
                AudioManager.Instance.PlaySFX("droppotato");
            }
            if (boxType == 2) // Kotak adalah wortel
            {
                carotCount += playerCaring;
                AudioManager.Instance.PlaySFX("dropcarrot");
            }
            playerScore += score;
            playerCaring = 0;
            vegetableType = 0;
            agent.AddReward(0.001f * score + gameManager.GetTimeBonus()*0.1f);
            return true;

        }
        if (vegetableType != 0)
        {
            agent.AddReward(-0.00001f * score);
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
        Vector3 dirToCarotToBox = (carotBox.transform.localPosition - transform.localPosition).normalized;
        return new PlayerProperties(isRed, isDoublePointActive, FearField.activeSelf, isDoublePointActive, FearField.activeSelf, playerSpeed, maxCapacity, playerCaring, vegetableType, potatoCount, carotCount, playerScore, playerPowerup, dirToPotatoToBox, dirToCarotToBox);
    }
    public void RandomizePosition()
    {
        float randomXPlayer = Random.Range(-5f, 5f);
        float randomYPlayer = Random.Range(-5f, 5f);
        transform.localPosition = new Vector3(randomXPlayer, randomYPlayer, transform.localPosition.z);

        float randomXPotatoBox = Random.Range(-5f, 5f);
        float randomYPotatoBox = Random.Range(-5f, 5f);
        Vector3 potatoBoxPosition = new Vector3(randomXPotatoBox, randomYPotatoBox, potatoBox.transform.localPosition.z);
        potatoBox.transform.localPosition = potatoBoxPosition;

        float randomXCarotBox = Random.Range(-5f, 5f);
        float randomYCarotBox = Random.Range(-5f, 5f);
        Vector3 carotBoxPosition = new Vector3(randomXCarotBox, randomYCarotBox, carotBox.transform.localPosition.z);
        carotBox.transform.localPosition = carotBoxPosition;

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
    public int CarotCount { get; private set; }
    public int PlayerScore { get; private set; }
    public Vector3 PlayerPowerUp { get; private set; }
    public Vector3 DirToPotatoBox { get; private set; }
    public Vector3 DirToCarotBox { get; private set; }

    public PlayerProperties(bool isRed, bool isDoublePointActive, bool isFearFieldActive, bool isDoublePointActive, bool isFearFieldActive, float speed, int capacity, int playerCaring, int vegetableType, int potatoCount, int carotCount, int playerScore, Vector3 playerPowerUp, Vector3 dirToPotatoBox, Vector3 dirToCarotBox)
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