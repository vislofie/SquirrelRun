using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private Item _item;
    public Item AttachedItem => _item;
}
