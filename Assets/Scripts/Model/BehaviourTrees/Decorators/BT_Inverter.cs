using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_Inverter : BT_DecoratorNode
{
    public BT_Inverter(BT_Node child) : base(child) {}

    public override BT_Result Tick(BT_AgentMemory am)
    {
        BT_Result result = TickChild(Child, am);

        if (result == BT_Result.FAILURE)
        {
            return BT_Result.SUCCESS;
        }
        else if (result == BT_Result.SUCCESS)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            return result;
        }
    }
}
