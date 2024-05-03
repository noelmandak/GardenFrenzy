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

    public bool isReceivedInput;
    private void Start()
    {
        player = GetComponent<Player>();
        powerUpManager = GetComponentInParent<PowerUpManager>();
        gameManager = GetComponentInParent<GameManager>();
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
            //    int totalVegetables = newPlayerProperties.CarotCount + newPlayerProperties.PotatoCount;
            //    float scoreChange = ((newPlayerProperties.PlayerScore - playerProperties.PlayerScore) / (gameManager.GetTotalVegetables() * 10)) * 0.01f; // Get rewarded for each point earned, to cover the possibility of different vegetables having different point values.
            //    float vegetableScore = (Mathf.Pow(totalVegetables, 2) / Mathf.Pow(gameManager.GetTotalVegetables(),2)) * 0.01f; // Ensuring the agent will collect vegetables until none are left.
            //    float totalReward = scoreChange + vegetableScore;
            //    AddReward(totalReward);
            //}   
        }
        playerProperties = newPlayerProperties;
        sensor.AddObservation(playerProperties.IsRed ? 0 : 1);
        sensor.AddObservation(playerProperties.IsDoublePointActive ? 1 : 0);
        sensor.AddObservation(playerProperties.IsFearFieldActive ? 1 : 0);
        sensor.AddObservation((playerProperties.PlayerCaring < playerProperties.MaxCapacity) ? 1 : 0); // is player can collect more
        sensor.AddObservation((playerProperties.VegetableType == 1) ? 1 : 0); // potato
        sensor.AddObservation((playerProperties.VegetableType == 2) ? 1 : 0); // carot
        sensor.AddObservation(playerProperties.PlayerScore);
        sensor.AddObservation(playerProperties.PlayerPowerUp);
        if (playerProperties.VegetableType== 1) sensor.AddObservation(new Vector2(playerProperties.DirToPotatoBox.x, playerProperties.DirToPotatoBox.y));
        else if (playerProperties.VegetableType == 2) sensor.AddObservation(new Vector2(playerProperties.DirToCarotBox.x, playerProperties.DirToCarotBox.y));
        else sensor.AddObservation(new Vector2(0, 0));
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
        if (actions.DiscreteActions[0] > 0 && (int)playerProperties.PlayerPowerUp[0] > 0)
        {
            powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[0], GetStar());
            player.CheckPowerupType(0);
        }
        if (actions.DiscreteActions[1] > 0 && (int)playerProperties.PlayerPowerUp[1] > 0) 
        {
            powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[1], GetStar());
            player.CheckPowerupType(1);
        }
        if (actions.DiscreteActions[2] > 0 && (int)playerProperties.PlayerPowerUp[2] > 0) 
        {
            powerUpManager.ActivatePower(playerProperties.IsRed, (int)playerProperties.PlayerPowerUp[2], GetStar());
            player.CheckPowerupType(2);
         }

        AddReward(-0.000001f);
    }

    public int GetStar()
    {
        return Random.Range(1, 4);
    }
}
