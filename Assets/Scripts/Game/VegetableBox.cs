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
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.MoveToBox(other.CompareTag("PlayerRed"), isCarrot ? 2 : 1, pointValue);
            }
        }
    }
}
