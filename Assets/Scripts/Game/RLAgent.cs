using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using Google.Protobuf.WellKnownTypes;
using UnityEngine.UIElements.Experimental;

public class RLAgent : Agent
{
    private Player player;
    private PlayerProperties playerProperties;
    [SerializeField]
    private InputActionReference move_action;

    public bool isReceivedInput;
    int timer = 1;
    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        timer += 1;
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        timer = 1;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        PlayerProperties newPlayerProperties = player.GetPlayerProperties();
        if (playerProperties!=null)
        {
            //if (newPlayerProperties.PlayerCaring > playerProperties.PlayerCaring) AddReward((newPlayerProperties.PlayerCaring - playerProperties.PlayerCaring) * 0.01f);
            //if (newPlayerProperties.PlayerCaring > playerProperties.PlayerCaring) AddReward((newPlayerProperties.PlayerCaring - playerProperties.PlayerCaring) * 0.01f);
            if (newPlayerProperties.PlayerScore > playerProperties.PlayerScore)
            {
                int totalVegetables = newPlayerProperties.CarotCount + newPlayerProperties.PotatoCount;
                float scoreChange = ((newPlayerProperties.PlayerScore - playerProperties.PlayerScore) / 80) * 0.1f; // Get rewarded for each point earned, to cover the possibility of different vegetables having different point values.
                float vegetableScore = (Mathf.Pow(totalVegetables, 2) / 64) * 0.01f; // Ensuring the agent will collect vegetables until none are left.
                float totalReward = scoreChange + vegetableScore;
                AddReward(totalReward);
            }   
        }
        playerProperties = newPlayerProperties;
        sensor.AddObservation(playerProperties.IsRed ? 0 : 1); 
        sensor.AddObservation(playerProperties.VegetableType);
        sensor.AddObservation(playerProperties.PlayerCaring);
        sensor.AddObservation(playerProperties.PotatoCount);
        sensor.AddObservation(playerProperties.CarotCount);
        sensor.AddObservation(playerProperties.PlayerScore);
        sensor.AddObservation(playerProperties.PlayerPowerUp);
        sensor.AddObservation(new Vector2(playerProperties.DirToPotatoBox.x, playerProperties.DirToPotatoBox.y));
        sensor.AddObservation(new Vector2(playerProperties.DirToCarotBox.x, playerProperties.DirToCarotBox.y));
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

        if (actions.DiscreteActions[0] > 0) player.ActivatePower(0);
        if (actions.DiscreteActions[1] > 0) player.ActivatePower(1);
        if (actions.DiscreteActions[2] > 0) player.ActivatePower(2);

    }
}
