using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public bool OnGround
    {
        get
        {
            return Physics.BoxCast(_collider.bounds.center, new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y / 2, _collider.bounds.extents.z), Vector3.down, Quaternion.identity, 0.1f);
        }
    }
    public bool WallNearby => _wallNearby;
    public bool Climbing => _climbing;
    public bool Running => _running;

    private GameObject _phantomPlayer;
    private Rigidbody _phantomRigidbody;

    private Rigidbody _rigidbody;
    private Collider _collider;

    [Header("Movement")]
    [SerializeField]
    private LayerMask _climbableLayers;
    [SerializeField]
    private float _normalSpeed = 3.0f;
    [SerializeField]
    private float _runningSpeed = 5.0f;
    [SerializeField]
    private float _jumpForce = 2.0f;
    [SerializeField]
    [Tooltip("The more this value is the more jump is going to be affected by player movement")]
    private float _jumpMovementDirAccelerator = 1.0f;
    [SerializeField][Tooltip("How smoothly main player is going to lerp into the phantom player(more value - less smoothly)")]
    private float _phantomLerpSmooth;

    private Vector3 _playerEulerVelocity;
    private Quaternion _beforePreparingForJumpRotation;

    private float _movementSpeed;

    private float _playerRotationY;
    private float _playerRotYVelocity;

    private bool _running;

    private bool _preparingForJump;

    [Header("Camera")]
    [SerializeField]
    private float _cameraSensitivity = 200.0f;
    [SerializeField][Tooltip("Speed of smoothing camera rotation in degrees / second")]
    private float _cameraSmoothValue = 30.0f;
    [SerializeField]
    private Vector2 _cameraClampNoClimbing = new Vector2(-75.0f, 75.0f);
    [SerializeField]
    private Vector2 _cameraClampClimbing = new Vector2(-15.0f, 90.0f);

    private Vector3 _cameraVelocity;
    private Vector3 _cameraEulerAngles;

    private Vector3 _movementDir;

    private float _cameraRotationX = 0.0f;
    private float _cameraRotationY = 0.0f;

    private RaycastHit _lastWallHit; // the last hit that was detected by a sphere cast
    private RaycastHit _hitAtStart; // hit that started the climbing

    private bool _wallNearby;
    private bool _climbing;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        CreatePhantomPlayer();

        _wallNearby = false;
        _climbing = false;

        _preparingForJump = false;

        _movementSpeed = _normalSpeed;
    }

    private void CreatePhantomPlayer()
    {
        _phantomPlayer = new GameObject("Phantom Player");
        _phantomPlayer.layer = LayerMask.NameToLayer("Player Phantom");

        _phantomRigidbody = _phantomPlayer.AddComponent<Rigidbody>();
        _phantomRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _phantomRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _phantomRigidbody.useGravity = false;
        _phantomRigidbody.velocity = Vector3.zero;

        BoxCollider collider = _phantomPlayer.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.3f, 0.3f, 0.9f);

        _phantomPlayer.SetActive(false);
    }
    private void ActivatePhantomPlayer()
    {
        _phantomPlayer.SetActive(true);

        _phantomPlayer.transform.position = transform.position;
        _phantomPlayer.transform.rotation = transform.rotation;
        
        _phantomRigidbody.useGravity = false;
        _phantomRigidbody.velocity = Vector3.zero;
    }
    private void HidePhantomPlayer()
    {
        _phantomPlayer.SetActive(false);
    }

    private void Update()
    {
        if (_climbing)
        {
            if (!_wallNearby)
            {
                StopClimbing();
            }
            else
            {
                if (!_preparingForJump)
                {
                    _phantomPlayer.transform.rotation = Quaternion.FromToRotation(_phantomPlayer.transform.up, _lastWallHit.normal) * _phantomPlayer.transform.rotation;
                }

                transform.position = Vector3.Lerp(transform.position, _phantomPlayer.transform.position, _phantomLerpSmooth * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, _phantomPlayer.transform.rotation, _phantomLerpSmooth * Time.deltaTime);
            }
        }
    }
    private void FixedUpdate()
    {
        CheckForWalls();
    }

    public void MoveOnInput(float horizontal, float vertical)
    {
        if (_preparingForJump)
            return;

        _movementDir = new Vector3(horizontal, 0, vertical).normalized;

        if (!_climbing)
            transform.Translate(_movementDir * Time.fixedDeltaTime * _movementSpeed);
        else
            _phantomPlayer.transform.Translate(_movementDir * Time.fixedDeltaTime * _movementSpeed);
    }
    public void RotateOnInput(float mouseX, float mouseY)
    {
       if (_climbing)
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, _cameraClampClimbing.x, _cameraClampClimbing.y);
            _playerRotationY = Mathf.SmoothDamp(_playerRotationY, mouseX * Time.deltaTime * _cameraSensitivity, 
                                                ref _playerRotYVelocity, _cameraSmoothValue * Time.deltaTime);

            _phantomPlayer.transform.Rotate(0, _playerRotationY, 0);
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

    public void Jump()
    {
        if (_climbing)
        {

        }
        else
        {
            _rigidbody.AddForce(((transform.rotation * _movementDir * _jumpMovementDirAccelerator).normalized + transform.up).normalized * _jumpForce);
        }
        
    }

    public void PrepareForJump()
    {
        _preparingForJump = true;

        _beforePreparingForJumpRotation = _phantomPlayer.transform.rotation;
        _cameraRotationX = 0;

        _phantomRigidbody.isKinematic = true;

        _phantomPlayer.transform.LookAt(_lastWallHit.point + _lastWallHit.normal * 5);
    }

    /// <summary>
    /// returns true if jumped and false if stayed on the surface
    /// </summary>
    /// <returns></returns>
    public bool UnPrepareForJump()
    {
        if (_preparingForJump)
        {
            _preparingForJump = false;
            _phantomRigidbody.isKinematic = false;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, _climbableLayers))
            {
                if (hit.collider == _hitAtStart.collider)
                {
                    return false;
                }
            }

            StopClimbing();

            _rigidbody.AddForce(Camera.main.transform.forward * 2 * _jumpForce);
            return true;
        }

        return false;
    }

    public void StartClimbing()
    {
        if (_lastWallHit.Equals(null))
            return;

        _hitAtStart = _lastWallHit;

        ActivatePhantomPlayer();
        
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;

        _playerEulerVelocity = Vector3.zero;

        _climbing = true;

        _phantomPlayer.transform.rotation = Quaternion.FromToRotation(_phantomPlayer.transform.up, _lastWallHit.normal) * _phantomPlayer.transform.rotation;
        _cameraRotationX = _cameraClampClimbing.y;
    }
    public void StopClimbing()
    {
        _rigidbody.useGravity = true;
        _climbing = false;

        HidePhantomPlayer();

        _playerEulerVelocity = Vector3.zero;

        transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;

        _preparingForJump = false;

    }

    public void StartRunning()
    {
        _movementSpeed = _runningSpeed;
        _running = true;
    }

    public void StopRunning()
    {
        _movementSpeed = _normalSpeed;
        _running = false;
    }

    private void CheckForWalls()
    {
        if (_preparingForJump)
            return;

        if (_climbing)
        {
            Ray[] rays =
            {
                new Ray(_phantomPlayer.transform.position, -_phantomPlayer.transform.up)
            };

            _wallNearby = false;

            foreach (Ray ray in rays)
            {
                if (Physics.SphereCast(ray, 0.25f, out _lastWallHit, 0.45f, _climbableLayers))
                {
                    _wallNearby = true;
                    break;
                }
            }
        }
        else
        {
            Ray[] rays =  
            { 
                new Ray(transform.position, transform.forward),
                new Ray(transform.position + transform.up * 0.15f, transform.forward),
                new Ray(transform.position - transform.up * 0.15f, transform.forward) 
            };

            _wallNearby = false;

            foreach (Ray ray in rays)
            {
                if (Physics.SphereCast(ray, 0.1f, out _lastWallHit, 0.45f, _climbableLayers))
                {
                    _wallNearby = true;
                    break;
                }
            }
        }
    }
}
