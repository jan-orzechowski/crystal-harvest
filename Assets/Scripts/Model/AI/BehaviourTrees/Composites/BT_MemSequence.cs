using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Wykonuje dzieci do pierwszej porażki lub błędu 
// Zapamiętuje węzeł w trakcie wykonywania (running) i przy wywołaniu zaczyna od niego
public class BT_MemSequence : BT_CompositeNode
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {
            am.SetInt(ID, "lastRunningNode", 0);
            am.SetRunning(ID, true);
        }
        
        for (int node = am.GetInt(ID, "lastRunningNode", 0); node < Children.Count; node++)
        {
            BT_Result result = Children[node].Tick(am);

            if (result == BT_Result.SUCCESS)
            {
                // Sprawdzamy następny
                continue;
            }
            else if(result == BT_Result.RUNNING)
            {
                am.SetInt(ID, "lastRunningNode", node);
                return result;
            }
            else
            {
                // Porażka lub błąd - zaczynamy od nowa
                am.SetRunning(ID, false);
                return result;
            }
        }

        am.SetRunning(ID, false);
        return BT_Result.SUCCESS;
    }
}