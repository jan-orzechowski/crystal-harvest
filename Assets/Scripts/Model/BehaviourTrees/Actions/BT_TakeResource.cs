using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_TakeResource : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null 
            && am.Character.HasResource == false
            && am.Character.Reservation.SourceStorage.GetAccessTile() == am.Character.CurrentTile)
        {
            if (am.Character.Reservation.SourceStorage.TransferFromStorage(am.Character.Reservation.Resource, am.Character))
            {
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;
    }
}
