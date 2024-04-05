using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AnotherPlayer : Agent
{
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(playerController.isRed);
        sensor.AddObservation(playerController.playerRedGO.transform.position);
        sensor.AddObservation(playerController.playerBlueGO.transform.position);

        sensor.AddObservation(playerController.playerRedType);
        sensor.AddObservation(playerController.playerRedCaring);
        sensor.AddObservation(playerController.playerRedKentangCount);
        sensor.AddObservation(playerController.playerRedWortelCount);
        sensor.AddObservation(playerController.playerRedScore);
        sensor.AddObservation(new Vector3(playerController.playerRedPowerUp[0], playerController.playerRedPowerUp[1], playerController.playerRedPowerUp[2]));

        sensor.AddObservation(playerController.playerBlueType);
        sensor.AddObservation(playerController.playerBlueCaring);
        sensor.AddObservation(playerController.playerBlueKentangCount);
        sensor.AddObservation(playerController.playerBlueWortelCount);
        sensor.AddObservation(playerController.playerBlueScore);
        sensor.AddObservation(new Vector3(playerController.playerBluePowerUp[0], playerController.playerBluePowerUp[1], playerController.playerBluePowerUp[2]));

    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);
        //Debug.Log(actions.ContinuousActions[1]);
        var x = actions.ContinuousActions[0];
        var y = actions.ContinuousActions[1];
        playerController.MovePlayer(playerController.isRed, new Vector2(x, y));
    }

    public void GameOver()
    {
        EndEpisode();
    }

    public void AddRewards(float reward)
    {
        AddReward(reward);
    }
}
