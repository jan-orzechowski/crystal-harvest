using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RemoveReservation : BT_Node 
{
    public override bool IsAction { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null)
        {
            if (am.Character.Reservation.SourceStorage != null)
                am.Character.Reservation.SourceStorage.RemoveResourceReservation(am.Character);

            am.Character.Reservation.TargetStorage.RemoveFreeSpaceReservation(am.Character);

            am.Character.ReservationUsed();
        }
        return BT_Result.SUCCESS;
    }
}
