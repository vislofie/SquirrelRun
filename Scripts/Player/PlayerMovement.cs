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
            Debug.Log(_collider.bounds.extents);
            return Physics.BoxCast(_collider.bounds.center, new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y / 2, _collider.bounds.extents.z), -transform.up, Quaternion.identity, 0.3f);
        }
    }
    public bool WallNearby => _wallNearby;
    public bool Climbing => _climbing;

    private Rigidbody _rigidbody;
    private Collider _collider;

    [Header("Movement")]
    [SerializeField]
    private float _movementSpeed = 3.0f;
    [SerializeField]
    private float _jumpForce = 2.0f;
    [SerializeField]
    [Tooltip("The more this value is the more jump is going to be affected by player movement")]
    private float _jumpMovementDirAccelerator = 1.0f;

    [Header("Camera")]
    [SerializeField]
    private float _cameraSensitivity = 200.0f;
    [SerializeField]
    private Vector2 _cameraClampNoClimbing = new Vector2(-75.0f, 75.0f);
    [SerializeField]
    private Vector2 _cameraClampClimbing = new Vector2(-15.0f, 90.0f);
    

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
        
        _wallNearby = false;
        _climbing = false;
    }

    private void FixedUpdate()
    {
        CheckForWalls();

        if (_climbing)
        {
            if (!_wallNearby)
                StopClimbing();

            transform.rotation = Quaternion.FromToRotation(transform.up, _lastWallHit.normal) * transform.rotation;
        }
    }

    public void MoveOnInput(float horizontal, float vertical)
    {
        _movementDir = new Vector3(horizontal, 0, vertical);
        transform.Translate(_movementDir * Time.deltaTime * _movementSpeed);
    }

    public void RotateOnInput(float mouseX, float mouseY)
    {
        if (_climbing)
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, _cameraClampClimbing.x, _cameraClampClimbing.y);
        }
        else
        {
            _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, _cameraClampNoClimbing.x, _cameraClampNoClimbing.y);
        }
        transform.Rotate(new Vector3(0, mouseX * _cameraSensitivity * Time.deltaTime, 0.0f));
        Camera.main.transform.localEulerAngles = new Vector3(_cameraRotationX, _cameraRotationY, 0.0f);
    }

    public void Jump()
    {
        _rigidbody.AddForce(((transform.rotation * _movementDir * _jumpMovementDirAccelerator).normalized + transform.up).normalized * _jumpForce);
    }

    public void Climb()
    {
        if (_lastWallHit.Equals(null))
            return;

        _hitAtStart = _lastWallHit;

        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _climbing = true;

        transform.rotation = Quaternion.FromToRotation(transform.up, _lastWallHit.normal) * transform.rotation;
    }

    public void StopClimbing()
    {
        _rigidbody.useGravity = true;
        _climbing = false;

        _cameraRotationY = 0.0f;

        transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
    }

    private void CheckForWalls()
    {
        if (_climbing)
        {
            Ray ray = new Ray(transform.position, -transform.up);
            _wallNearby = Physics.SphereCast(ray, 0.3f, out _lastWallHit, 0.45f);
        }
        else
        {
            Ray[] rays =  { new Ray(transform.position, transform.forward),
                        new Ray(transform.position + transform.up * 0.15f, transform.forward),
                        new Ray(transform.position - transform.up * 0.15f, transform.forward) };
            _wallNearby = false;

            foreach (Ray ray in rays)
            {
                if (Physics.SphereCast(ray, 0.1f, out _lastWallHit, 0.45f))
                {
                    _wallNearby = true;
                    break;
                }
            }
        }
    }

}
