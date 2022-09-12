using UnityEngine;

public class PlayerSenses : MonoBehaviour
{
    [SerializeField][Tooltip("Raycast mask for checking collectables")]
    private LayerMask _raycastMask;

    /// <summary>
    /// Returns true if there are any collectables nearby and false if otherwise
    /// </summary>
    /// <param name="collectable"></param>
    /// <returns></returns>
    public bool GetClosestCollectable(out Collectable collectable)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5.0f, _raycastMask);

        return hit.collider.TryGetComponent(out collectable);
    }
}
