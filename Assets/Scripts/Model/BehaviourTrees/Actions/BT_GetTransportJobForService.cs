using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTransportJobForService : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null) { return BT_Result.FAILURE; }
        if (am.Service == null) { return BT_Result.FAILURE; }

        World world = GameManager.Instance.World;

        if (world.GetReservationForFillingInput(am.Character, am.Service.InputStorage)
            || (am.Service.InputStorage.RequiresEmptying
                && world.GetReservationForEmptying(am.Character, am.Service.InputStorage)))
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
