using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BT_GoTo : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        Tile goal = GetDestinationTile(am);

        if (goal == null) return BT_Result.FAILURE;

        if (am.IsRunning(ID) == false)
        {
            if (am.Character.SetNewDestination(goal, true))
            {
                am.Character.SetLastTileRotation(GetDestinationTileRotation(am));

                Debug.Log("Nowe GoTo: " + goal.ToString());

                am.StartRunning(ID);
                return BT_Result.RUNNING;
            }
            else return BT_Result.FAILURE;           
        }
        else
        {
            WhileRunning(am);

            Debug.Log("GoTo: " + goal.ToString());

            if (am.Character.CurrentTile == goal)
            {
                am.StopRunning(ID);
                Debug.Log("GoTo: sukces");
                return BT_Result.SUCCESS;
            }

            if (am.Character.IsTileMarkedAsInaccessible(goal) || am.Character.DestinationTile != goal)
            {
                Debug.Log("GoTo: pole niedostępne");
                am.StopRunning(ID);
                return BT_Result.FAILURE;
            }

            return BT_Result.RUNNING;
        }
    }

    public abstract void WhileRunning(BT_AgentMemory am);
    public abstract Tile GetDestinationTile(BT_AgentMemory am);
    public abstract Rotation GetDestinationTileRotation(BT_AgentMemory am);
}