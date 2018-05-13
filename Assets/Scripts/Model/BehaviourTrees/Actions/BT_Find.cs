using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BT_Find : BT_Node 
{
    static float timeoutAfterNullResults = 5f;

    class BT_FindNodeData
    {
        public IAccessible PotentialDestination;
        public bool UseSecondAccessTile;
    }

    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        //Debug.Log("Find Precondition: " + (am.HasTimerElapsed(ID)));
        return (am.HasTimerElapsed(ID)
                && IsSearchNeededCondition(am));
    }

    public override void Activate(BT_AgentMemory am)
    {
        //Debug.Log("Find " + ID + " - activate");
        am.SetObject(ID, new BT_FindNodeData());
    }

    public override void Deactivate(BT_AgentMemory am)
    {
        //Debug.Log("Find " + ID + " - deactivate");
        am.SetObject(ID, null);
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        BT_FindNodeData data = am.GetObject(ID) as BT_FindNodeData;
        if (data == null) return BT_Result.FAILURE;

        if (data.PotentialDestination == null)
        {
            data.UseSecondAccessTile = false;

            data.PotentialDestination = GetPotentialDestination(am, data.UseSecondAccessTile);

            if (data.PotentialDestination == null)
            {
                am.SetTimer(ID, timeoutAfterNullResults);
                return BT_Result.FAILURE;
            }

            if (am.Character.IsTileMarkedAsInaccessible(data.PotentialDestination.GetAccessTile(false))
                && am.Character.IsTileMarkedAsInaccessible(data.PotentialDestination.GetAccessTile(true)) == false)
                data.UseSecondAccessTile = true;

            am.Character.SetNewDestination(data.PotentialDestination.GetAccessTile(data.UseSecondAccessTile), false);

            return BT_Result.RUNNING;
        }
        else
        {
            Tile accessTile = data.PotentialDestination.GetAccessTile(data.UseSecondAccessTile);

            if (am.Character.IsTileMarkedAsInaccessible(accessTile))
            {
                // Debug.Log("pole niedostępne");
                data.PotentialDestination = null;
                return BT_Result.RUNNING;
            }

            if (am.Character.DestinationTile != accessTile)
            {
                // Debug.Log("Szukanie - błędne pole");
                data.PotentialDestination = null;
                return BT_Result.RUNNING;
            }

            // Debug.Log("szukanie trwa");

            if (am.Character.CurrentTile == accessTile)
            {
                // Znaleziono
                Found(am, data.PotentialDestination, data.UseSecondAccessTile);
                return BT_Result.SUCCESS;
            }

            if (am.Character.Path != null && am.Character.Path.IsReady)
            {
                if (am.Character.Path.IsImpossible)
                {
                    // Ścieżka niemożliwa
                    // Debug.Log("Szukanie - ścieżka niemożliwa");
                    data.PotentialDestination = null;
                    return BT_Result.RUNNING;
                }
                else
                {
                    // Znaleziono
                    Found(am, data.PotentialDestination, data.UseSecondAccessTile);
                    return BT_Result.SUCCESS;
                }
            }
            else
            {
                return BT_Result.RUNNING;
            }
        }
    }

    public abstract bool IsSearchNeededCondition(BT_AgentMemory am);
    public abstract IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile);
    public abstract void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed);
}
