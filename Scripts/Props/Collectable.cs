using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private ItemID _itemID;
    public ItemID ItemID => _itemID;

    public GameObject Prefab { get; private set; }

    public void Initialize(ItemID itemID)
    {
        _itemID = itemID;
    }

    private void Start()
    {
        Prefab = ItemManager.Instance.GetItemPrefab(_itemID);
    }
}
