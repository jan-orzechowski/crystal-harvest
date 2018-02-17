using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_HasService : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
