using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RotateTowardsWorkplace : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null)
        {
            return BT_Result.ERROR;
        }
        else
        {
            am.Character.SetLastTileRotation(am.Workplace.GetAccessTileRotation());
            return BT_Result.SUCCESS;
        }
    }
}
