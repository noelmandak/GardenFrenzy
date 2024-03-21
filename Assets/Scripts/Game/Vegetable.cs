using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public bool isCarrot = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerRed") || other.CompareTag("PlayerBlue"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                if (player.CollectVegetable(other.CompareTag("PlayerRed"), isCarrot ? 2 : 1))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
