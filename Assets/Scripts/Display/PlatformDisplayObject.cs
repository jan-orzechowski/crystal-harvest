using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlatformDisplayObject : SelectableDisplayObject
{
    PlatformTopsPrefabs Prefabs;

    GameObject currentTop;

    void Awake()
    {
        Prefabs = GameManager.Instance.PlatformTopsPrefabs;

        if (currentTop == null)
        {
            currentTop = SimplePool.Spawn(Prefabs.Top_4Sides, this.transform.position, Quaternion.identity);
            currentTop.transform.SetParent(this.transform);
        }
    }

    void Update()
    {
        
    }
}
