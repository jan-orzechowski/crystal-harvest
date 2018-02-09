using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_Timer : BT_DecoratorNode
{
    float timer;

    public BT_Timer (float timer, BT_Node child) : base(child)
    {
        this.timer = timer;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {
            am.SetFloat(ID, "timer", timer);
            am.SetRunning(ID, true);
        }

        timer = am.GetFloat(ID, "timer", 0f);
        timer -= am.DeltaTime;
        am.SetFloat(ID, "timer", timer);

        if (timer <= 0)
        {
            am.SetRunning(ID, false);
            return Child.Tick(am);
        }
        else
        {
            return BT_Result.RUNNING;
        }
    }
}
