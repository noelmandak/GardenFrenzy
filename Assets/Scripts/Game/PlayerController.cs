using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.ScrollRect;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 300.0f;
    private Rigidbody2D rig;

    [SerializeField]
    private InputActionReference move_action;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = move_action.action.ReadValue<Vector2>();
        Debug.Log(movement);
        MoveObject(movement);
    }
    private void MoveObject(Vector2 movement)
    {
        rig.velocity = playerSpeed * Time.deltaTime * movement;
        Debug.Log("VELOCITY");
        Debug.Log(rig.velocity);
    }
}
