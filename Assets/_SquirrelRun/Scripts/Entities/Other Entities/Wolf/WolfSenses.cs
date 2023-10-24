using System.Collections.Generic;
using UnityEngine;

public class WolfSenses : MonoBehaviour
{
    public Transform PrioritizedVisibleTarget
    {
        get
        {
            if (_visibleObjects.Count != 0)
                return _visibleObjects[0];
            return null;
        }
    }

    [Tooltip("Layers of objects that this wolf should see and save in his head")]
    [SerializeField]
    private LayerMask _viewMask;
    [Tooltip("Layers of objects that this wolf should not see through")]
    [SerializeField]
    private LayerMask _obstaclesMask;

    [Tooltip("Radius in meters that represents this wolf's view distance")]
    [SerializeField]
    private float _viewRadius;
    [Tooltip("Angle in degrees that reperesents this wolf's field of view")]
    [SerializeField]
    private float _viewAngle;

    public float ViewRadius => _viewRadius;
    public float ViewAngle => _viewAngle;

    private List<Transform> _visibleObjects = new List<Transform>();

    /// <summary>
    /// Find visible objects for this wolf
    /// </summary>
    public void FindVisibleObjects()
    {
        _visibleObjects.Clear();
        Collider[] objectsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _viewMask);

        foreach (Collider collider in objectsInViewRadius)
        {
            Transform targetTransform = collider.transform;
            Vector3 dirToTarget = (collider.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distance = Vector3.Distance(transform.position, targetTransform.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distance, _obstaclesMask))
                {
                    _visibleObjects.Add(targetTransform);
                }
            }
        }
    }

    /// <summary>
    /// Tells whether the object is inside view radius and is not hidden by any obstacles
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool InsideTheZone(Transform obj)
    {
        Vector3 dirToTarget = obj.position - transform.position;
        float distance = Vector3.Distance(transform.position, obj.position);

        return !Physics.Raycast(transform.position, dirToTarget, distance, _obstaclesMask);
    }

    /// <summary>
    /// Creates a direction vector from a given angle
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
