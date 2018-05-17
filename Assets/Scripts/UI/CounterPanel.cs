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
            TimerText.text = "00 : 00";
        }
        else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(GameManager.Instance.World.TimeLeft);
            string minutesText = (timeSpan.Minutes < 10) ? "0" + timeSpan.Minutes : timeSpan.Minutes.ToString();
            string secondsText = (timeSpan.Seconds < 10) ? "0" + timeSpan.Seconds : timeSpan.Seconds.ToString();

            TimerText.text = (minutesText + " : " + secondsText);
        }

        int currentAmount = world.AllResources[world.ResourceToGather];
        int goalAmount = world.AmountToGather;
        string newCounter = currentAmount + " / " + goalAmount;
        CounterText.text = newCounter;
    }
}
