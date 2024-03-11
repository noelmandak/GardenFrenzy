using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.ScrollRect;

public class PlayerController : MonoBehaviour
{
    public bool isRed = true;
    public float playerSpeed = 300.0f;
    public Rigidbody2D playerRed;
    public Rigidbody2D playerBlue;
    private Rigidbody2D player;

    [SerializeField]
    private InputActionReference move_action;

    public CameraFollow cameraFollow;

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = move_action.action.ReadValue<Vector2>();
        MoveObject(movement);
    }
    private void MoveObject(Vector2 movement)
    {
        player = isRed ? playerRed : playerBlue;
        player.velocity = playerSpeed * Time.deltaTime * movement;
        cameraFollow.UpdateCamera(player.transform);
    }

    public void SetPlayerSpeed(float speed)
    {
        playerSpeed = speed;
    }

    public float GetPlayerSpeed()
    {
        return playerSpeed;
    }

    public void ChangePlayer()
    {
        isRed = !isRed;
    }
}
