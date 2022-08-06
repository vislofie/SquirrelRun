using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private Transform _slotPosition;

    private GameObject _currentItem;

    public bool SlotEmpty { get; private set; }

    private void Awake()
    {
        SlotEmpty = true;
    }

    public void FillSlot(Collectable collectable)
    {
        if (SlotEmpty)
        {
            _currentItem = collectable.gameObject;

            _currentItem.transform.SetParent(_slotPosition);
            _currentItem.transform.localPosition = Vector3.zero;
            _currentItem.transform.localRotation = Quaternion.identity;

            Rigidbody rb = _currentItem.GetComponent<Rigidbody>();
            rb.isKinematic = true;

            _currentItem.GetComponent<Collider>().enabled = false;

            SlotEmpty = false;
        }
    }

    public void FreeSlot()
    {
        if (!SlotEmpty)
        {
            Rigidbody rb = _currentItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            _currentItem.GetComponent<Collider>().enabled = true;

            _currentItem.transform.parent = null;

            _currentItem = null;

            SlotEmpty = true;
        }
    }
}
