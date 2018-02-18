using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_ReserveJob : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace.IsJobFree() == false)
        {
            return BT_Result.FAILURE;
        }
        else if (am.Workplace.ReserveJob(am.Character))
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
