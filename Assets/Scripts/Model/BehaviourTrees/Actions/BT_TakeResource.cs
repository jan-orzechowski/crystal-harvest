using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_TakeResource : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Reservation != null 
            && am.Character.Resource == 0
            && am.Reservation.SourceStorage.GetAccessTile() == am.Character.CurrentTile)
        {
            if (am.Reservation.SourceStorage.TransferFromStorage(am.Reservation.Resource, am.Character))
            {
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;
    }
}
