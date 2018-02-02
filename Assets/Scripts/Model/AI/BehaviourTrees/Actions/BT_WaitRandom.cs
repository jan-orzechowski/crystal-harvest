using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_WaitRandom : BT_ActionNode
{
    float minWaitingTime;
    float maxWaitingTime;
    BT_Wait childWaitNode;

    public BT_WaitRandom(float minWaitingTime, float maxWaitingTime)
    {
        this.minWaitingTime = minWaitingTime;
        this.maxWaitingTime = maxWaitingTime;
        childWaitNode = new BT_Wait(0);
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {            
            childWaitNode.ChangeWaitingTime(
                UnityEngine.Random.Range(minWaitingTime, maxWaitingTime));
            am.SetRunning(ID, true);
        }

        BT_Result result = childWaitNode.Tick(am);

        if (result != BT_Result.RUNNING)
        {
            am.SetRunning(ID, false);
        }

        return result;
    }

}
