using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_Work : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if(am.Workplace == null || am.Workplace.GetAccessTile() != am.Character.CurrentTile)
        {
            return BT_Result.ERROR;
        }
        else
        {
            bool result = am.Workplace.Work(am.DeltaTime, am.Character);
            if (result)
            {
                if(am.Workplace.ProductionStarted == false)
                {
                    return BT_Result.SUCCESS;
                }
                else
                {
                    return BT_Result.RUNNING;
                }                
            }
            else
            {
                return BT_Result.FAILURE;
            }
        }
    }
}
