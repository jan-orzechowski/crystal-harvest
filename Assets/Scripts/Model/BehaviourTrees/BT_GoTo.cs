using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BT_GoTo : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.IsRunning(ID) == false)
        {
            Tile goal = GetDestinationTile(am);
            if (goal == null)
            {
                return BT_Result.FAILURE;
            }
            else
            {
                am.SetTile(ID, "goal", goal);
                am.Character.SetNewDestination(goal);
                am.Character.SetLastTileRotation(GetDestinationTileRotation(am));

                am.SetRunning(ID, true);
                return BT_Result.RUNNING;
            }
        }
        else
        {
            Tile goal = am.GetTile(ID, "goal");
            if (goal == null)
            {
                return BT_Result.FAILURE;
            }

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

    public abstract Tile GetDestinationTile(BT_AgentMemory am);

    public abstract Rotation GetDestinationTileRotation(BT_AgentMemory am);
}
