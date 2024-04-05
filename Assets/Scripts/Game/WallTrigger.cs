using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRed"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                //player.AddReward(-0.5f);
                Debug.Log("player red hit the wall");
            }
        } else if (other.CompareTag("PlayerBlue"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                //player.anotherPlayer.AddRewards(-0.5f);
                Debug.Log("player blue hit the wall");
            }
        }
    }
}
