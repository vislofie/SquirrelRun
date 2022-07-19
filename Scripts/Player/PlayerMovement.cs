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
            return Physics.BoxCast(_collider.bounds.center, _collider.bounds.extents / 2, -transform.up, transform.rotation, _collider.bounds.extents.y) ||
                   Physics.BoxCast(_collider.bounds.center + transform.rotation * new Vector3(0, 0, _collider.bounds.extents.z), _collider.bounds.extents / 2, -transform.up, transform.rotation, _collider.bounds.extents.y) ||
                   Physics.BoxCast(_collider.bounds.center - transform.rotation * new Vector3(0, 0, _collider.bounds.extents.z), _collider.bounds.extents / 2, -transform.up, transform.rotation, _collider.bounds.extents.y);
            
        }
    }
    public bool WallNearby => _wallNearby;
    public bool Climbing => _climbing;

    private Rigidbody _rigidbody;
    private Collider _collider;

    [SerializeField]
    private float _movementSpeed = 3.0f;
    [SerializeField]
    private float _cameraSensitivity = 200.0f;
    [SerializeField]
    private float _jumpForce = 2.0f;

    [SerializeField][Tooltip("The more this value is the more jump is going to be affected by player movement")]
    private float _jumpMovementDirAccelerator = 1.0f;
    private Vector3 _movementDir;
    private float _cameraRotationX = 0.0f;

    private RaycastHit _lastWallHit;

    private bool _wallNearby;
    private bool _climbing;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        _wallNearby = false;
        _climbing = false;
    }

    private void Update()
    {
        CheckForWalls();
    }

    public void MoveOnInput(float horizontal, float vertical)
    {
        _movementDir = new Vector3(horizontal, 0, vertical);
        transform.Translate(_movementDir * Time.deltaTime * _movementSpeed);
    }

    public void RotateOnInput(float mouseX, float mouseY)
    {
        transform.Rotate(new Vector3(0, mouseX * _cameraSensitivity * Time.deltaTime, 0.0f));

        _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseY * Time.deltaTime * _cameraSensitivity, -75, 75);
        Camera.main.transform.localEulerAngles = new Vector3(_cameraRotationX, 0.0f, 0.0f);
    }

    public void Jump()
    {
        _rigidbody.AddForce(((transform.rotation * _movementDir * _jumpMovementDirAccelerator).normalized + transform.up).normalized * _jumpForce);
    }

    public void Climb()
    {

    }

    private void CheckForWalls()
    {
        Ray[] rays =  { new Ray(transform.position, transform.forward),
                        new Ray(transform.position + transform.up * 0.3f, transform.forward),
                        new Ray(transform.position - transform.up * 0.3f, transform.forward)};

        _wallNearby = false;

        foreach (Ray ray in rays)
        {
            if (Physics.SphereCast(ray, 0.3f, out _lastWallHit, 0.3f))
            {
                _wallNearby = true;
            }
        }
    }
}