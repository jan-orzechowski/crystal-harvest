using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_Wait : BT_ActionNode
{
    float waitingTime;

    public BT_Wait(float waitingTime)
    {
        this.waitingTime = waitingTime;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {       
        if (am.IsRunning(ID) == false)
        {
            // Debug.Log("BT_Wait - początek. Czas: " + waitingTime + "s.");
            am.SetFloat(ID, "timer", waitingTime);
            am.SetRunning(ID, true);
        }

        float timer = am.GetFloat(ID, "timer", 0f);
        timer -= am.DeltaTime;
        am.SetFloat(ID, "timer", timer);

        if (timer <= 0)
        {
            am.SetRunning(ID, false);
            // Debug.Log("BT_Wait - koniec");
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.RUNNING;
        }
    }

    public void ChangeWaitingTime(float newTime)
    {
        waitingTime = newTime;
    }
}
