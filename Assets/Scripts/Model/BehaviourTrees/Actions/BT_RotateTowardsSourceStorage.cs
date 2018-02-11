using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RotateTowardsSourceStorage : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if(am.Reservation == null || am.Reservation.SourceStorage == null)
        {
            return BT_Result.ERROR;
        }
        else
        {
            am.Character.SetLastTileRotation(am.Reservation.SourceStorage.GetAccessTileRotation());
            return BT_Result.SUCCESS;
        }
    }
}
