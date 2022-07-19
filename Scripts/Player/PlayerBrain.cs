using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerBrain : MonoBehaviour
{
    private PlayerMovement _movement;

    private float _horizontal;
    private float _vertical;

    private float _mouseX;
    private float _mouseY;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        _movement.RotateOnInput(_mouseX, _mouseY);

        if (Input.GetKeyDown(KeyCode.Space) && _movement.OnGround)
        {
            _movement.Jump();
        }
    }

    private void FixedUpdate()
    {
        _movement.MoveOnInput(_horizontal, _vertical);
    }
}
