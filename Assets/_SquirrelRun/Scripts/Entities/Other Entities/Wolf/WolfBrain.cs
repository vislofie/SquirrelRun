using UnityEngine;

[RequireComponent(typeof(WolfSenses))]
public class WolfBrain : MonoBehaviour
{
    private WolfSenses _wolfSenses;

    private void Awake()
    {
        _wolfSenses = GetComponent<WolfSenses>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        _wolfSenses.FindVisibleObjects();
    }
}
