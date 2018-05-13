using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsAtWorkplace : BT_Node 
{
    public override bool Activates { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null)
        {
            return BT_Result.ERROR;
        }

        if (am.Workplace.GetAccessTile() == am.Character.CurrentTile)
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
