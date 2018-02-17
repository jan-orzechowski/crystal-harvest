﻿using System.Collections;
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

        if (am.Workplace.InputStorage.IsFilled == false || am.Workplace.OutputStorage.IsEmpty == false)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
