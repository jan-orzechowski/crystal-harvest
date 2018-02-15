using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsWorkplaceReadyForProduction : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null)
        {
            return BT_Result.ERROR;
        }

        if (am.Workplace.MissingResourcesCount > 0 || am.Workplace.ResourcesToRemoveCount > 0)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
