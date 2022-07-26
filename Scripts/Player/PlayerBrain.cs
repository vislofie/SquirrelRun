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
        TakeInput();
        MoveByInput();
    }

    private void TakeInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");
    }

    private void MoveByInput()
    {
        _movement.RotateOnInput(_mouseX, _mouseY);

        if (_movement.Climbing)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                _movement.StopClimbing();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _movement.PrepareForJump();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                _movement.UnPrepareForJump();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && _movement.WallNearby)
            {
                _movement.Climb();
            }

            if (_movement.OnGround && Input.GetKeyDown(KeyCode.Space))
            {
                _movement.Jump();
            }
        }
    }

    private void FixedUpdate()
    {
        _movement.MoveOnInput(_horizontal, _vertical);
    }
}
