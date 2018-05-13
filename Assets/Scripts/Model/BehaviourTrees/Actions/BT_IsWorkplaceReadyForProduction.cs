using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsWorkplaceReadyForProduction : BT_Node
{
    public override bool Activates { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null)
        {
            return BT_Result.ERROR;
        }

        if (am.Workplace.Halted == true
            || am.Workplace.InputStorage.RequiresEmptying
            || am.Workplace.InputStorage.AreRequirementsMet == false 
            || am.Workplace.OutputStorage.IsEmpty == false)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
