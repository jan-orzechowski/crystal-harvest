using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Sprawdza dzieci do pierwszego wyniku, który nie jest failure
public class BT_Priority : BT_CompositeNode
{ 
    public override BT_Result Tick(BT_AgentMemory am)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            BT_Result result = TickChild(Children[i], am);

            if (result == BT_Result.FAILURE)
            {
                continue;
            }
            else
            {
                return result;
            }
        }
        return BT_Result.FAILURE;
    }
}