using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsServiceReady : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null)
        {
            return BT_Result.ERROR;
        }

        if (am.Service.InputStorage.IsFilled == false)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
