using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_RepeatUntilFail : BT_DecoratorNode
{
    public BT_RepeatUntilFail(BT_Node child) : base(child) { }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        BT_Result result;

        do
        {
            result = Child.Tick(am);
        }
        while (result == BT_Result.SUCCESS || result == BT_Result.RUNNING);

        return result;
    }
}
