using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalAxis { get; private set; }
    public float VerticalAxis { get; private set; }
    public float MouseHorizontalAxis { get; private set; }
    public float MouseVerticalAxis { get; private set; }

    public event Action OnSprintPressed = delegate {};
    public event Action OnSprintUp = delegate {};

    public event Action OnWoundCreatePressed = delegate {};

    public event Action OnClimbPressed = delegate {};
    public event Action OnClimbUp = delegate {};

    public event Action OnJumpPressed = delegate {};
    public event Action OnJumpUp = delegate {};

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");
        MouseHorizontalAxis = Input.GetAxis("Mouse X");
        MouseVerticalAxis = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.L))
        {
            OnWoundCreatePressed();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnClimbPressed();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            OnClimbUp();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpPressed();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnSprintPressed();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnSprintUp();
        }
    }
}
