using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerBrain : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerChars _chars;

    private Camera _mainCamera;

    private float _horizontal;
    private float _vertical;

    private float _mouseX;
    private float _mouseY;

    private float _restExp;
    private bool _restReset;
    
    [SerializeField]
    private int _targetFrameRate;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _chars = GetComponent<PlayerChars>();

        _mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        _restReset = false;

        Application.targetFrameRate = _targetFrameRate;
    }

    private void Start()
    {
        _chars.HP = 10000;
        _chars.Stamina = 10000;
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
        bool movingAtAll = (Mathf.Abs(_horizontal) + Mathf.Abs(_vertical)) != 0;

        if (!movingAtAll)
        {
            if (!_restReset)
            {
                _restExp = 0.0f;
                _restReset = true;
            }

            _restExp += Time.deltaTime * _chars.RestExpValue;
        }
        else if (movingAtAll)
        {
            if (_restReset)
            {
                _restReset = false;
                _restExp = 0.0f;
            }
        }

        _chars.Stamina += Time.deltaTime * _chars.StaminaRestVlaue * 0.5f * Mathf.Exp(_restExp);





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
            else if (Input.GetKeyUp(KeyCode.Space) && _chars.Stamina > _chars.StaminaCostForFromSurfaceJump)
            {
                if (_movement.UnPrepareForJump())
                    _chars.Stamina -= _chars.StaminaCostForFromSurfaceJump;
            }

            if (movingAtAll)
                _chars.Stamina -= Time.deltaTime * _chars.StaminaCostForClimbing;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && _movement.WallNearby)
            {
                _movement.StartClimbing();
            }

            if (_movement.OnGround && Input.GetKeyDown(KeyCode.Space) && _chars.Stamina > _chars.StaminaCostForGroundJump)
            {
                _movement.Jump();
                _chars.Stamina -= _chars.StaminaCostForGroundJump;
            }

            _chars.Stamina += Time.deltaTime * _chars.StaminaRestVlaue;
        }

        _movement.RotateOnInput(_mouseX, _mouseY);

        if (_chars.Stamina < 1.0f)
        {
            _movement.StopClimbing();
            _movement.StopRunning();
        }





        if (Input.GetKeyDown(KeyCode.LeftShift) && movingAtAll)
        {
            _movement.StartRunning();
            _mainCamera.fieldOfView = 120;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _movement.StopRunning();
            _mainCamera.fieldOfView = 90;
        }

        if (_movement.Running)
            _chars.Stamina -= Time.deltaTime * _chars.StaminaCostForRunning;
    }

    private void FixedUpdate()
    {
        _movement.MoveOnInput(_horizontal, _vertical);
    }
}
