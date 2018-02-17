using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RotateTowardsService : BT_Node     
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null)
        {
            return BT_Result.ERROR;
        }
        else
        {
            am.Character.SetLastTileRotation(am.Service.GetAccessTileRotation());
            return BT_Result.SUCCESS;
        }
    }
}
