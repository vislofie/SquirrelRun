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
}
