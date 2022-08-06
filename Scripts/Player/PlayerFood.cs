using UnityEngine;

public class PlayerFood : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How much hunger is going to be added per second\n" +
                             "This value is multiplied by Time.deltaTime")]
    private float _hungerRate;
    public float HungerRate => _hungerRate;

    

    private void Awake()
    {
        
    }

    
}
