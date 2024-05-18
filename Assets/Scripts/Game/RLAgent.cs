using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class RLAgent : Agent
{
    private Player player;
    private PlayerProperties playerProperties;
    [SerializeField]
    private InputActionReference move_action;
    private PowerUpManager powerUpManager;
    private GameManager gameManager;
    private VegetableSpawner vegetableSpawner;

    public bool isReceivedInput;
    private void Start()
    {
        player = GetComponent<Player>();
        powerUpManager = GetComponentInParent<PowerUpManager>();
        gameManager = GetComponentInParent<GameManager>();
        vegetableSpawner = GetComponentInParent<VegetableSpawner>();
    }


    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        PlayerProperties newPlayerProperties = player.GetPlayerProperties();
        if (playerProperties!=null)
        {
            //if (newPlayerProperties.PlayerCaring > playerProperties.PlayerCaring) AddReward((newPlayerProperties.PlayerCaring - playerProperties.PlayerCaring) * 0.01f);
            //if (newPlayerProperties.PlayerCaring > playerProperties.PlayerCaring) AddReward((newPlayerProperties.PlayerCaring - playerProperties.PlayerCaring) * 0.01f);
            //if (newPlayerProperties.PlayerScore > playerProperties.PlayerScore)
            //{
            //    int totalVegetables = newPlayerProperties.CarrotCount + newPlayerProperties.PotatoCount;
            //    float scoreChange = ((newPlayerProperties.PlayerScore - playerProperties.PlayerScore) / (gameManager.GetTotalVegetables() * 10)) * 0.01f; // Get rewarded for each point earned, to cover the possibility of different vegetables having different point values.
            //    float vegetableScore = (Mathf.Pow(totalVegetables, 2) / Mathf.Pow(gameManager.GetTotalVegetables(),2)) * 0.01f; // Ensuring the agent will collect vegetables until none are left.
            //    float totalReward = scoreChange + vegetableScore;
            //    AddReward(totalReward);
            //}   

            // Mendapatkan vektor arah ke potato terdekat
            
        }
        playerProperties = newPlayerProperties;
        sensor.AddObservation(playerProperties.IsRed ? 0 : 1);
        sensor.AddObservation(playerProperties.IsDoublePointActive ? 1 : 0);
        sensor.AddObservation(playerProperties.IsFearFieldActive ? 1 : 0);
        sensor.AddObservation((playerProperties.PlayerCaring < playerProperties.MaxCapacity) ? 1 : 0); // is player can collect more
        sensor.AddObservation((playerProperties.VegetableType == 1) ? 1 : 0); // potato
        sensor.AddObservation((playerProperties.VegetableType == 2) ? 1 : 0); // carrot
        sensor.AddObservation(playerProperties.PlayerScore);
        sensor.AddObservation(playerProperties.PlayerPowerUp);
        sensor.AddObservation(getVegetableDirection());
        float[] vegetabledir = getVegetableDirection();
        Debug.Log($"{vegetabledir[0]} {vegetabledir[1]}  {vegetabledir[2]}  {vegetabledir[3]}  {vegetabledir[4]}  {vegetabledir[5]} ");
        sensor.AddObservation(new Vector2(playerProperties.DirToPotatoBox.x, playerProperties.DirToPotatoBox.y));
        sensor.AddObservation(new Vector2(playerProperties.DirToCarrotBox.x, playerProperties.DirToCarrotBox.y));
        //if (playerProperties.VegetableType == 1) sensor.AddObservation(new Vector2(playerProperties.DirToPotatoBox.x, playerProperties.DirToPotatoBox.y));
        //else if (playerProperties.VegetableType == 2) sensor.AddObservation(new Vector2(playerProperties.DirToCarrotBox.x, playerProperties.DirToCarrotBox.y));
        //else sensor.AddObservation(new Vector2(0, 0));
    }

    private float[] getVegetableDirection()
    {
        List<GameObject> nearestPotatoes = vegetableSpawner.GetNearestVegetable(playerProperties.PlayerPosition, 1, false);
        Vector3 dirToNearestPotato = Vector3.zero;
        if (nearestPotatoes.Count > 0)
        {
            dirToNearestPotato = (nearestPotatoes[0].transform.localPosition - playerProperties.PlayerPosition)/1000f;
        }

        // Mendapatkan vektor arah ke carrot terdekat
        List<GameObject> nearestCarrots = vegetableSpawner.GetNearestVegetable(playerProperties.PlayerPosition, 1, true);
        Vector3 dirToNearestCarrot = Vector3.zero;
        if (nearestCarrots.Count > 0)
        {
            dirToNearestCarrot = (nearestCarrots[0].transform.localPosition - playerProperties.PlayerPosition)/1000f;
        }

        // Membuat array observasi
        float[] observationArray = new float[6];
        observationArray[0] = dirToNearestPotato.x;
        observationArray[1] = dirToNearestPotato.y;
        observationArray[2] = nearestPotatoes.Count > 0 ? 1 : 0; // 1 jika ada potato, 0 jika tidak
        observationArray[3] = dirToNearestCarrot.x;
        observationArray[4] = dirToNearestCarrot.y;
        observationArray[5] = nearestCarrots.Count > 0 ? 1 : 0; // 1 jika ada carrot, 0 jika tidak

        return observationArray;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (isReceivedInput)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            Vector2 movement = move_action.action.ReadValue<Vector2>();
            continuousActions[0] = movement.x;
            continuousActions[1] = movement.y;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var x = actions.ContinuousActions[0];
        var y = actions.ContinuousActions[1];
        player.MovePlayer(new Vector2(x, y));
        PlayerProperties playerProperties = player.GetPlayerProperties();
        //if (actions.DiscreteActions[0] > 0 && (int)playerProperties.PlayerPowerUp[0] > 0)
        //{
        //    powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[0], GetStar());
        //    player.CheckPowerupType(0);
        //}
        //if (actions.DiscreteActions[1] > 0 && (int)playerProperties.PlayerPowerUp[1] > 0) 
        //{
        //    powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[1], GetStar());
        //    player.CheckPowerupType(1);
        //}
        //if (actions.DiscreteActions[2] > 0 && (int)playerProperties.PlayerPowerUp[2] > 0) 
        //{
        //    powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[2], GetStar());
        //    player.CheckPowerupType(2);
        // }

    }

    public int GetStar()
    {
        return Random.Range(1, 4);
    }
}
