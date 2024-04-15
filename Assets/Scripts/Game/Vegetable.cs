using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public bool isCarrot = false;
    int redCount = 0;
    int blueCount = 0;


    void OnTriggerStay2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        RLAgent agent = other.GetComponent<RLAgent>();
        if (player == null)
        {
            player = other.GetComponentInParent<Player>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
            agent = other.GetComponentInParent<RLAgent>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
        }
        if (player != null)
        {
            if (player.GetPlayerProperties().IsRed) redCount++;
            else blueCount++;
            if (redCount > 10) agent.AddReward(redCount * -0.000001f);
            if (blueCount > 10) agent.AddReward(blueCount * -0.000001f);

            if (player.CollectVegetable(isCarrot ? 2 : 1))
            {
                Destroy(gameObject);
            }
        }
    }
}
