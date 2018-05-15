using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_RemoveWorkplaceIfCannotReserve : BT_Node 
{
    public override bool Activates { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace != null 
            && am.Workplace.CanReserveJob(am.Character) == false)
        {
            am.Workplace = null;
            am.Character.SetNewDestination(am.Character.CurrentTile, false, null, this);
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
