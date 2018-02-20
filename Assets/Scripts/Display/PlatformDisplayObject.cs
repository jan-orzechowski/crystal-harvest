using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlatformDisplayObject : SelectableDisplayObject
{
    PlatformTopsPrefabs Prefabs;

    GameObject currentTop;

    void Start()
    {
        Prefabs = GameManager.Instance.PlatformTopsPrefabs;
    }

    void Update()
    {
        if(currentTop == null)
        {
            currentTop = SimplePool.Spawn(Prefabs.Top_4Sides, this.transform.position, Quaternion.identity);
            currentTop.transform.parent.SetParent(this.transform);
        }
    }
}
