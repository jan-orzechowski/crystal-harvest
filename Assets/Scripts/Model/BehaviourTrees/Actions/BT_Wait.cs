using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_Wait : BT_Node
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
            am.SetFloat(ID, "timer", waitingTime);
            am.StartRunning(ID);
        }

        float timer = am.GetFloat(ID, "timer", 0f);
        timer -= am.DeltaTime;
        am.SetFloat(ID, "timer", timer);

        if (timer <= 0)
        {
            am.StopRunning(ID);
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
