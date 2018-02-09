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
            Debug.Log("Nie ma miejsca pracy!");
            return BT_Result.ERROR;
        }

        if (am.Workplace.MissingResourcesCount > 0 || am.Workplace.OutputResourcesCount > 0)
        {
            Debug.Log("Brakuje zasobów lub miejsca!");
            return BT_Result.FAILURE;
        }
        else
        {
            Debug.Log("Można pracować!");
            return BT_Result.SUCCESS;
        }
    }
}
