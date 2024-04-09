using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public bool isCarrot = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player == null) player = other.GetComponentInParent<Player>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
        if (player != null)
        {
            if (player.CollectVegetable(isCarrot ? 2 : 1))
            {
                Destroy(gameObject);
            }
        }
    }
}
