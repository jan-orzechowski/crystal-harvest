using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_WaitRandom : BT_Node
{
    float minWaitingTime;
    float maxWaitingTime;
    float waitingTime;

    public BT_WaitRandom(float minWaitingTime, float maxWaitingTime)
    {
        this.minWaitingTime = minWaitingTime;
        this.maxWaitingTime = maxWaitingTime;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {
            waitingTime = UnityEngine.Random.Range(minWaitingTime, maxWaitingTime);
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

}
