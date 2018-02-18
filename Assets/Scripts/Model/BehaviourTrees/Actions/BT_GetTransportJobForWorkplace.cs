using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTransportJobForWorkplace : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null) { return BT_Result.FAILURE; }
        if (am.Workplace == null) { return BT_Result.FAILURE; }

        World world = GameManager.Instance.World;

        if (world.GetReservationForFillingInput(am.Character, am.Workplace))
        {
            return BT_Result.SUCCESS;
        }
        else if (world.GetReservationForHandlingOutput(am.Character, am.Workplace))
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
