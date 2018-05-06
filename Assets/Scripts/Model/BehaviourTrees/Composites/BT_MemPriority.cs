using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_MemPriority : BT_CompositeNode
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {
            am.SetInt(ID, "lastRunningNode", 0);
            am.StartRunning(ID);
        }

        for (int node = am.GetInt(ID, "lastRunningNode", 0); node < Children.Count; node++)
        {
            BT_Result result = TickChild(Children[node], am);

            if (result == BT_Result.FAILURE)
            {
                continue;
            }
            else if (result == BT_Result.RUNNING)
            {
                am.SetInt(ID, "lastRunningNode", node);
                return result;
            }            
            else
            {
                am.StopRunning(ID);
                return result;
            }
        }

        am.StopRunning(ID);
        return BT_Result.FAILURE;
    }
}