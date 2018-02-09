using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_GoTo : BT_Node
{
    string goalVarName;

    public BT_GoTo (string goalVarName) : base()
    {
        this.goalVarName = goalVarName;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        Tile goal = am.GetGlobalTile(goalVarName);
        if (goal == null)
        {
            return BT_Result.FAILURE;
        }

        if (am.IsRunning(ID) == false)
        {
            am.Character.SetNewDestination(goal);
            am.SetRunning(ID, true);
            return BT_Result.RUNNING;
        }
        else
        {
            if (am.Character.CurrentTile == goal)
            {
                am.SetRunning(ID, false);
                return BT_Result.SUCCESS;
            }

            if (am.Character.DestinationTile != goal)
            {
                am.SetRunning(ID, false);
                return BT_Result.FAILURE;
            }

            return BT_Result.RUNNING;
        }
    }
}
