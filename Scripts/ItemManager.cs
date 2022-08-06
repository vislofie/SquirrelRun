using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemID { Acorn = 0 };
public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ItemManager>();
            return _instance;
        }
    }

    private Dictionary<ItemID, GameObject> _itemPrefabs = new Dictionary<ItemID, GameObject>();

    private void Awake()
    {
        GameObject[] prefabGameObjects = Resources.LoadAll<GameObject>("Prefabs/Collectables");

        foreach (GameObject prefab in prefabGameObjects)
        {
            _itemPrefabs.Add((ItemID)int.Parse(prefab.name.Split('_')[0]), prefab);
        }
    }

    public GameObject GetItemPrefab(ItemID itemID)
    {
        if (_itemPrefabs.ContainsKey(itemID))
            return _itemPrefabs[itemID];

        Debug.LogWarning("No item in resources folder with " + itemID + " ID!");
        return null;
    }
}
