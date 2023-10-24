using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName="Inventory/Food Item")]
public class FoodItem : Item
{
    [SerializeField]
    [Tooltip("Whether this food item can be peeled or not")]
    private bool _peelable;
    public bool Peelable => _peelable;

    [SerializeField]
    [Tooltip("Or UnPeeled one if you prefer")]
    private Mesh _normalMesh;
    public Mesh NormalMesh => _normalMesh;

    [SerializeField]
    [Tooltip("Only use if this object can be peeled")]
    private Mesh _peeledMesh;
    public Mesh PeeledMesh => _peeledMesh;

    [SerializeField]
    [Tooltip("How much hunger is going to be substracted from the hunger bar\n" +
             "Scale from 1 to 100; 100 being completely removes hunger")]
    private float _hungerReduceValue;
    public float HungerReduceValue => _hungerReduceValue;

    [SerializeField]
    [Tooltip("How much time is it going to take to peel this food from its shell\n" +
             "Or how much time is it going to take to clean it from something")]
    private float _timeToPeel;
    public float TimeToPeel => _timeToPeel;

    [SerializeField]
    [Tooltip("How much time is it going to take to eat this food item")]
    private float _timeToEat;
    public float TimeToEat => _timeToEat;
}
