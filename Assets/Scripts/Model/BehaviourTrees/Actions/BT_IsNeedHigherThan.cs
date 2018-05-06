using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsNeedHigherThan : BT_Node 
{
    public override bool IsAction { get { return false; } }

    string need;
    float threshold;

    public BT_IsNeedHigherThan(string need, float threshold) : base()
    {
        this.need = need;
        this.threshold = threshold;
    }
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Needs.ContainsKey(need) 
            && am.Character.Needs[need] > threshold)
        {
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
