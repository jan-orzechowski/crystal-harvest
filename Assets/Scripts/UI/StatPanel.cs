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

    public Text HumanCounter;
    public Text RobotCounter;

    void Update()
    {
        foreach (ResourceCounterInfo rci in ResourceCounters)
        {
            int count = GameManager.Instance.World.AllResources[rci.ResourceID];
            rci.Counter.text = count.ToString();
        }
        HumanCounter.text = GameManager.Instance.World.HumanNumber.ToString();
        RobotCounter.text = GameManager.Instance.World.RobotNumber.ToString();
    }
}
