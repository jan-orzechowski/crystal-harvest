using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_HasResourceForReservation : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Reservation == null) { return BT_Result.ERROR; }
        if (am.Reservation.Resource == am.Character.Resource)
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
