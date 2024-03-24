using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int powerUpType = 0; // 1 = red, 2 = blue, 3 = purple, 4 = yellow

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerBlue"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                if (player.CollectPowerUp(other.CompareTag("PlayerRed"), powerUpType))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
