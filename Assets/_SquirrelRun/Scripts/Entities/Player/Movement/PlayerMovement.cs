using UnityEngine;

// TODO: Separate climbing mechanics from player movement into a different class

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhantomController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public bool OnGround
    {
        get
        {
            return Physics.BoxCast(_collider.bounds.center, new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y / 2, _collider.bounds.extents.z), Vector3.down, Quaternion.identity, 0.1f);
        }
    }

    public bool WallNearby { get; private set; }
    public bool Climbing { get; private set; }

    public bool Running { get; private set; }

    private PhantomController _phantomController;

    private PlayerInput _input;

    private Rigidbody _rigidbody;
    private Collider _collider;

    [Header("Movement")]
    [SerializeField]
    private float _normalSpeed = 3.0f;
    [SerializeField]
    private float _runningSpeed = 5.0f;
    
    [SerializeField][Tooltip("How smoothly main player is going to lerp into the phantom player(more value - less smoothly)")]
    private float _phantomLerpSmooth;

    [SerializeField]
    [Tooltip("The more this value is the more jump is going to be affected by player movement")]
    private float _jumpMovementDirAccelerator = 1.0f;

    [Header("Rotation")]
    [SerializeField]
    private LayerMask _climbableLayers;
    [SerializeField]
    private float _jumpForce = 2.0f;

    [SerializeField]
    private float _cameraSensitivity = 200.0f;
    [SerializeField]
    [Tooltip("Speed of smoothing camera rotation in degrees / second")]
    private float _cameraSmoothValue = 30.0f;
    [SerializeField]
    private Vector2 _cameraClampNoClimbing = new Vector2(-75.0f, 75.0f);
    [SerializeField]
    private Vector2 _cameraClampClimbing = new Vector2(-15.0f, 90.0f);

    private Vector3 _playerEulerVelocity;

    private Vector3 _cameraVelocity;
    private Vector3 _cameraEulerAngles;

    private float _playerRotationY;
    private float _playerRotYVelocity;

    private float _cameraRotationX = 0.0f;

    private bool _preparingForJump;

    private RaycastHit _lastWallHit; // the last hit that was detected by a sphere cast
    private RaycastHit _hitAtStart; // hit that started the climbing

    private float _movementSpeed;

    private Vector3 _movementDir;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _phantomController = GetComponent<PhantomController>();
        _input = GetComponent<PlayerInput>();

        _movementSpeed = _normalSpeed;

        _input.OnSprintPressed += StartRunning;
        _input.OnSprintUp += StopRunning;

        _input.OnClimbPressed += StartClimbing;
        _input.OnClimbUp += StopClimbing;

        _input.OnJumpPressed += Jump;
        _input.OnJumpUp += UnPrepareForJump;

        WallNearby = false;
        Climbing = false;

        _preparingForJump = false;
    }

    private void Update()
    {
        if (Climbing)
        {
            if (!WallNearby)
            {
                StopClimbing();
            }
            else
            {
                if (!_preparingForJump)
                {
                    _phantomController.PhantomTransform.rotation = Quaternion.FromToRotation(_phantomController.PhantomTransform.up, _lastWallHit.normal) * _phantomController.PhantomTransform.rotation;
                }

                transform.position = Vector3.Lerp(transform.position, _phantomController.PhantomTransform.position, _phantomLerpSmooth * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, _phantomController.PhantomTransform.rotation, _phantomLerpSmooth * Time.deltaTime);
            }
        }
    }
    private void FixedUpdate()
    {
        MoveOnInput();
        RotateOnInput();

        CheckForWalls();
    }

    /// <summary>
    /// Moves according to given input
    /// </summary>
    /// <param name="horizontal">horizontal axis of WASD</param>
    /// <param name="vertical">vertical axis of WASD</param>
    private void MoveOnInput()
    {
        float horizontal = _input.HorizontalAxis;
        float vertical = _input.VerticalAxis;

        if (_preparingForJump)
            return;

        _movementDir = new Vector3(horizontal, 0, vertical).normalized;

        if (!Climbing)
            transform.Translate(_movementDir * Time.fixedDeltaTime * _movementSpeed);
        else
            _phantomController.PhantomTransform.Translate(_movementDir * Time.fixedDeltaTime * _movementSpeed);
    }

    /// <summary>
    /// Rotates according to given input
    /// </summary>
    private void RotateOnInput()
    {
        float mouseX = _input.MouseHorizontalAxis;
        float mouseY = _input.MouseVerticalAxis;

        if (Climbing)
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, _cameraClampClimbing.x, _cameraClampClimbing.y);
            _playerRotationY = Mathf.SmoothDamp(_playerRotationY, mouseX * Time.deltaTime * _cameraSensitivity,
                                                ref _playerRotYVelocity, _cameraSmoothValue * Time.deltaTime);

            _phantomController.PhantomTransform.Rotate(0, _playerRotationY, 0);
        }
        else
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, _cameraClampNoClimbing.x, _cameraClampNoClimbing.y);
            _playerRotationY = mouseX * Time.deltaTime * _cameraSensitivity * _cameraSmoothValue;

            transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, transform.localEulerAngles + new Vector3(0, _playerRotationY, 0),
                                                            ref _playerEulerVelocity, _cameraSmoothValue * Time.deltaTime);
        }

        _cameraEulerAngles = Vector3.SmoothDamp(_cameraEulerAngles, new Vector3(_cameraRotationX, 0.0f, 0.0f),
                                                ref _cameraVelocity, _cameraSmoothValue * Time.deltaTime);

        Camera.main.transform.localEulerAngles = _cameraEulerAngles;
    }

    /// <summary>
    /// Changes movement speed to run speed
    /// </summary>
    private void StartRunning()
    {
        _movementSpeed = _runningSpeed;
        Running = true;
    }

    /// <summary>
    /// Changes movement speed to walk speed
    /// </summary>
    private void StopRunning()
    {
        _movementSpeed = _normalSpeed;
        Running = false;
    }

    /// <summary>
    /// Initialization of climbing
    /// </summary>
    private void StartClimbing()
    {
        if (_lastWallHit.Equals(null))
            return;

        _hitAtStart = _lastWallHit;

        _phantomController.ActivatePhantom();

        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;

        Climbing = true;

        _phantomController.PhantomTransform.rotation = Quaternion.FromToRotation(_phantomController.PhantomTransform.up, _lastWallHit.normal) * _phantomController.PhantomTransform.rotation;
    }

    /// <summary>
    /// Reverts values to stop climbing
    /// </summary>
    private void StopClimbing()
    {
        _rigidbody.useGravity = true;
        Climbing = false;

        _phantomController.DeactivatePhantom();

        transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        _preparingForJump = false;

    }

    /// <summary>
    /// Fuckin jumps idk
    /// </summary>
    private void Jump()
    {
        if (Climbing)
        {
            PrepareForJump();
        }
        else
        {
            _rigidbody.AddForce(((transform.rotation * _movementDir * _jumpMovementDirAccelerator).normalized + transform.up).normalized * _jumpForce);
        }
    }

    /// <summary>
    /// Rotates player to direction of a jump and blocks any movement
    /// </summary>
    private void PrepareForJump()
    {
        _preparingForJump = true;

        _phantomController.PhantomRigidbody.isKinematic = true;

        _phantomController.PhantomTransform.LookAt(_lastWallHit.point + _lastWallHit.normal * 5);
    }

    private void UnPrepareForJump()
    {
        if (_preparingForJump)
        {
            _preparingForJump = false;
            _phantomController.PhantomRigidbody.isKinematic = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _climbableLayers))
            {
                if (hit.collider == _hitAtStart.collider)
                {
                    return;
                }
            }

            StopClimbing();

            _rigidbody.AddForce(Camera.main.transform.forward * 2 * _jumpForce);
        }
    }

    /// <summary>
    /// Checks if there are any walls touching the player
    /// </summary>
    private void CheckForWalls()
    {
        if (_preparingForJump)
            return;

        if (Climbing)
        {
            WallNearby = IsThereWallBelowBody();
        }
        else
        {
            WallNearby = IsThereWallInFrontBody();
        }
    }

    private bool IsThereWallBelowBody()
    {
        Ray[] rays =
            {
                new Ray(_phantomController.PhantomTransform.position, - _phantomController.PhantomTransform.up)
            };

        foreach (Ray ray in rays)
        {
            if (Physics.SphereCast(ray, 0.25f, out _lastWallHit, 0.45f, _climbableLayers))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsThereWallInFrontBody()
    {
        Ray[] rays =
            {
                new Ray(transform.position, transform.forward),
                new Ray(transform.position + transform.up * 0.15f, transform.forward),
                new Ray(transform.position - transform.up * 0.15f, transform.forward)
            };

        foreach (Ray ray in rays)
        {
            if (Physics.SphereCast(ray, 0.1f, out _lastWallHit, 0.45f, _climbableLayers))
            {
                return true;
            }
        }

        return false;
    }


}
