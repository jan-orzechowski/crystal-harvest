using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RotateTowardsTargetStorage : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation == null || am.Character.Reservation.TargetStorage == null)
        {
            return BT_Result.ERROR;
        }
        else
        {
            am.Character.SetLastTileRotation(am.Character.Reservation.TargetStorage.GetAccessTileRotation());
            return BT_Result.SUCCESS;
        }
    }
}
