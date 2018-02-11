using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_DepositResource : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Reservation != null
            && am.Character.HasResource
            && am.Reservation.TargetStorage.GetAccessTile() == am.Character.CurrentTile)
        {
            if (am.Reservation.TargetStorage.TransferToStorage(am.Reservation.Resource, am.Character))
            {
                am.ReservationUsed();
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;
    }
}
