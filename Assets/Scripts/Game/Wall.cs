using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if ((other.CompareTag("PlayerRed")) || (other.CompareTag("PlayerBlue")))
        {
            RLAgent agent = other.GetComponent<RLAgent>();
            if (agent == null)
            {
                agent = other.GetComponentInParent<RLAgent>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
            }
            if (agent != null)
            {
                agent.AddReward(-0.000001f);
            }
        }
    }
}
