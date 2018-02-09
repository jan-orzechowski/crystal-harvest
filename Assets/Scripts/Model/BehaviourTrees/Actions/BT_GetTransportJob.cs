using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTransportJob : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {        
        if (am.Reservation != null) { return BT_Result.FAILURE; }
        
        World world = GameManager.Instance.World;

        ResourceReservation newReservation = world.GetReservationForFillingInput(am.Character);

        if (newReservation == null)
        {
            newReservation = world.GetReservationForHandlingOutput(am.Character);
        }

        if (newReservation == null)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            am.SetNewReservation(newReservation);
            return BT_Result.SUCCESS;
        }
    }
}
