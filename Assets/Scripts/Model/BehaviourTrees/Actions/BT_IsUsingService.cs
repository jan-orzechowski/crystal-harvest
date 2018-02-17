using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsUsingService : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.UsingService)
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
