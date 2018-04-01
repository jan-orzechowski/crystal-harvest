using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CounterPanel : MonoBehaviour 
{
    public Text TimerText;
    public Text CounterText;
   
    void Update()
    {
        World world = GameManager.Instance.World;

        string newTimer = "10:00";
        TimerText.text = newTimer;

        int currentAmount = world.AllResources[world.ResourceToGather];
        int goalAmount = world.AmountToGather;
        string newCounter = currentAmount + " / " + goalAmount;
        CounterText.text = newCounter;
    }
}
