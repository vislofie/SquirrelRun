using UnityEngine;

public class TriggerReceiver : MonoBehaviour
{
    private Collectable _closestCollectable = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _closestCollectable))
        {
            Debug.Log("TouchingCollectable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _closestCollectable = null;
    }

    public bool GetClosestCollectable(out Collectable collectable)
    {
        collectable = _closestCollectable;
        return _closestCollectable != null;
    }
}
