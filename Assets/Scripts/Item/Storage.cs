using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : SingletonMonoBehaviour<Storage>
{
    public Dictionary<Item,int> items;

    private void Start()
    {
        items = new Dictionary<Item, int>();
    }

}
