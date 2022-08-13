using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Transform _slotPosition;

    private Collectable _currentCollectable;
    public Collectable CurrentCollectable => _currentCollectable;

    public bool SlotEmpty { get; private set; }

    private void Awake()
    {
        SlotEmpty = true;
    }

    /// <summary>
    /// Fills inventory slot with given collectable
    /// </summary>
    /// <param name="collectable">collectable to fill slot with</param>
    public void FillSlot(Collectable collectable)
    {
        if (SlotEmpty)
        {
            _currentCollectable = collectable;

            _currentCollectable.transform.SetParent(_slotPosition);
            _currentCollectable.transform.localPosition = Vector3.zero;
            _currentCollectable.transform.localRotation = Quaternion.identity;

            Rigidbody rb = _currentCollectable.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            _currentCollectable.GetComponent<Collider>().enabled = false;

            SlotEmpty = false;
        }
    }

    /// <summary>
    /// Frees slot of the inventory
    /// </summary>
    public void FreeSlot()
    {
        if (!SlotEmpty)
        {
            Rigidbody rb = _currentCollectable.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            _currentCollectable.GetComponent<Collider>().enabled = true;

            _currentCollectable.transform.parent = null;

            _currentCollectable = null;

            SlotEmpty = true;
        }
    }

    /// <summary>
    /// Peels item in the slot if it's a food item
    /// </summary>
    public void PeelCurrent()
    {
        if (_currentCollectable.AttachedItem != null && _currentCollectable.AttachedItem.GetType() == typeof(FoodItem))
        {
            CollectableFood col = _currentCollectable as CollectableFood;
            if (col.Peeled) return;

            FoodItem foodItem = _currentCollectable.AttachedItem as FoodItem;

            col.Peel();
            _currentCollectable.GetComponent<MeshFilter>().mesh = foodItem.PeeledMesh;
        }
    }

    /// <summary>
    /// Eats item in the slot if it's a food item and was peeled
    /// </summary>
    /// <returns></returns>
    public float EatCurrent()
    {
        float hungerReduce = (_currentCollectable.AttachedItem as FoodItem).HungerReduceValue;

        Destroy(_currentCollectable.gameObject);

        SlotEmpty = true;

        return hungerReduce;
    }
}
