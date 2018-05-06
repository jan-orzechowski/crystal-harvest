﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsServiceReady : BT_Node
{
    public override bool IsAction { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null)
        {
            return BT_Result.ERROR;
        }

        if (am.Service.InputStorage.AreRequirementsMet == false)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
