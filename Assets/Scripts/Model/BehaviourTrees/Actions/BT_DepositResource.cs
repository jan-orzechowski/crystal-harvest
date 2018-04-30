using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_DepositResource : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null
            && am.Character.HasResource
            && (am.Character.Reservation.TargetStorage.GetAccessTile(false) == am.Character.CurrentTile
                || am.Character.Reservation.TargetStorage.GetAccessTile(true) == am.Character.CurrentTile))
        {
            if (am.Character.Reservation.TargetStorage.TransferToStorage(am.Character.Reservation.Resource, am.Character))
            {
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;
    }
}
