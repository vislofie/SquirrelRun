using UnityEngine;

public class PhantomController : MonoBehaviour
{
    public Transform PhantomTransform { get; private set; }
    public Rigidbody PhantomRigidbody { get; private set; }

    private GameObject _phantomObj;

    private void Awake()
    {
        CreatePhantomPlayer();
    }

    /// <summary>
    /// Creates an instance of a phantom player
    /// </summary>
    private void CreatePhantomPlayer()
    {
        _phantomObj = new GameObject("Phantom Player");
        _phantomObj.layer = LayerMask.NameToLayer("Player Phantom");

        PhantomTransform = _phantomObj.transform;

        PhantomRigidbody = _phantomObj.AddComponent<Rigidbody>();
        PhantomRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        PhantomRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        PhantomRigidbody.useGravity = false;
        PhantomRigidbody.velocity = Vector3.zero;

        BoxCollider collider = _phantomObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.3f, 0.3f, 0.9f);

        _phantomObj.SetActive(false);
    }

    /// <summary>
    /// Enables phantom player in the hierarchy
    /// </summary>
    public void ActivatePhantom()
    {
        _phantomObj.SetActive(true);

        _phantomObj.transform.position = transform.position;
        _phantomObj.transform.rotation = transform.rotation;

        PhantomRigidbody.useGravity = false;
        PhantomRigidbody.velocity = Vector3.zero;
    }
    
    /// <summary>
    /// Hides phantom player in the hierarchy
    /// </summary>
    public void DeactivatePhantom()
    {
        _phantomObj.SetActive(false);
    }
}
