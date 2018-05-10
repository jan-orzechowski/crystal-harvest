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

        ResourceReservation newReservation = world.GetReservationForFillingInput(am.Character, am.Workplace.InputStorage);
        if (newReservation == null)
            newReservation = world.GetReservationForEmptying(am.Character, am.Workplace.OutputStorage);
        if (newReservation == null && am.Workplace.InputStorage.RequiresEmptying)
            newReservation = world.GetReservationForEmptying(am.Character, am.Workplace.InputStorage);

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
