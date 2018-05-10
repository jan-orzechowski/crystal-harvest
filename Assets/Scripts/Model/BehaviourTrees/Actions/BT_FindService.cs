using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindService : BT_Find 
{
    string need;

    public BT_FindService(string need) : base()
    {
        this.need = need;
    }

    public override bool IsSearchNeededCondition(BT_AgentMemory am)
    {
        return (am.Service == null
                || am.Character.IsTileMarkedAsInaccessible(
                   am.Service.GetAccessTile(am.UseServiceSecondAccessTile)));
    }

    public override IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile)
    {
        return GameManager.Instance.World.GetClosestAvailableService(need, am.Character);
    }

    public override void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed)
    {
        am.Service = matchingCandidate as Service;
        am.UseServiceSecondAccessTile = secondAccessTileUsed;
    }    
}
