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

        float timeLeft = GameManager.Instance.World.TimeLeft;
        if (timeLeft <= 0f)
        {
            TimerText.text = "0 : 0";
        }
        else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.World.TimeLeft);
            TimerText.text = (timeSpan.Minutes + " : " + timeSpan.Seconds);
        }

        int currentAmount = world.AllResources[world.ResourceToGather];
        int goalAmount = world.AmountToGather;
        string newCounter = currentAmount + " / " + goalAmount;
        CounterText.text = newCounter;
    }
}
