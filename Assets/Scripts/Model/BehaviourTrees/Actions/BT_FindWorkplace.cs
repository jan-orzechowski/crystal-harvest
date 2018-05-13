using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindWorkplace : BT_Find 
{
    public override bool IsSearchNeededCondition(BT_AgentMemory am)
    {
        return (am.Workplace == null                                                        
                || am.Character.IsTileMarkedAsInaccessible(
                    am.Workplace.GetAccessTile(am.UseWorkplaceSecondAccessTile)));
    }

    public override IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile)
    {
        return GameManager.Instance.World.GetAvailableWorkplace(am.Character);
    }

    public override void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed)
    {
        am.Workplace = matchingCandidate as IWorkplace;
        am.UseWorkplaceSecondAccessTile = secondAccessTileUsed;
    }
}
