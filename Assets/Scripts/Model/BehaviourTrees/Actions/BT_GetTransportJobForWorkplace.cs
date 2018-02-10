using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTransportJobForWorkplace : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Reservation != null) { return BT_Result.FAILURE; }
        if (am.Workplace == null) { return BT_Result.FAILURE; }

        World world = GameManager.Instance.World;

        ResourceReservation newReservation;

        newReservation = world.GetReservationForFillingInput(am.Character, am.Workplace);

        if (newReservation == null)
        {
            newReservation = world.GetReservationForHandlingOutput(am.Character, am.Workplace);
        }

        if (newReservation == null)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            am.SetNewReservation(newReservation);
            //Debug.Log("Nowa rezerwacja: " + newReservation.Resource);
            return BT_Result.SUCCESS;
        }
    }
}
