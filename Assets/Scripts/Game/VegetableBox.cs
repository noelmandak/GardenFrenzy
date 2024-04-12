using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableBox : MonoBehaviour
{
    public int pointValue = 0;
    public bool isCarrot = false;
    public bool isRed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("PlayerRed") && isRed) || (other.CompareTag("PlayerBlue") && !isRed))
        {
            Player player = other.GetComponent<Player>();
            if (player == null)
            {
                player = other.GetComponentInParent<Player>(); // Example: Fearfield have the PlayerTag but not have the Player component, although its parent does.
            }
            if (player != null)
            {
                player.MoveToBox(isCarrot ? 2 : 1, pointValue);
            }
        }
    }
}
