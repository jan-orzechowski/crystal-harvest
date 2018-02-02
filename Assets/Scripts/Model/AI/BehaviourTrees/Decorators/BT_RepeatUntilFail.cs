using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_RepeatUntilFail : BT_DecoratorNode
{
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
