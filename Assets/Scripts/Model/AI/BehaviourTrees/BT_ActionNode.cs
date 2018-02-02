using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_ActionNode : BT_Node
{
    // Kod testowy - akcja zostanie wykonana po 20 tickach
    public override BT_Result Tick(BT_AgentMemory am)
    {
        Debug.Log("ActionNode - debug");

        if (am.IsRunning(ID) == false)
        {
            am.SetInt(ID, "count", 0);
            am.SetRunning(ID, true);
        }
        else
        {
            int count = am.GetInt(ID, "count", 0);
            count++;          
            if (count >= 20)
            {
                // Działanie wykonane
                Debug.Log("ActionNode - debug - koniec!");
                am.SetRunning(ID, false);
                return BT_Result.SUCCESS;
            }
            else
            {
                am.SetInt(ID, "count", count);
            }
        }

        return BT_Result.RUNNING;
    }
}
