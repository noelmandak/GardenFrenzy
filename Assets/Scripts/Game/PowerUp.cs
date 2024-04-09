using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int powerUpType = 0; // 1 = red, 2 = blue, 3 = purple, 4 = yellow

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) player = other.GetComponentInParent<Player>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
        if (player != null)
        {
            if (player.CollectPowerUp(powerUpType))
            {
                Destroy(gameObject);
            }
        }
    }
}
