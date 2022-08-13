using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableFood : Collectable
{
    private bool _peeled;
    public bool Peeled => _peeled;

    public void Peel()
    {
        _peeled = true;
    }
}
