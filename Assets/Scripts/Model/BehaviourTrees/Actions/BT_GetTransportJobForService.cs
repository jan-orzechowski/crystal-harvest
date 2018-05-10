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

        ResourceReservation newReservation = world.GetReservationForFillingInput(am.Character, am.Service.InputStorage);
        if (newReservation == null && am.Service.InputStorage.RequiresEmptying)
        {
            newReservation = world.GetReservationForEmptying(am.Character, am.Service.InputStorage);
        }
        if (newReservation != null)
        {
            am.Character.SetNewReservation(newReservation);
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }        
    }
}
