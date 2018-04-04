using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatPanel : MonoBehaviour 
{
    [Serializable]
    public class ResourceCounterInfo
    {
        public Text Counter;
        public int ResourceID;
    }

    public ResourceCounterInfo[] ResourceCounters;

    void Update()
    {
        foreach (ResourceCounterInfo rci in ResourceCounters)
        {
            int count = GameManager.Instance.World.AllResources[rci.ResourceID];
            rci.Counter.text = count.ToString();
        }   
    }
}
