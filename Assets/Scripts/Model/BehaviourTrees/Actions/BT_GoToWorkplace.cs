using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToWorkplace : BT_GoTo
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Workplace != null);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        if (am.Workplace.Building.Prototype.IsNaturalDeposit)
        {
            if (GameManager.Instance.World.ReserveNaturalDeposit(am.Character, am.Workplace))
            {
                return;
            }
            else
            {
                am.Character.WorkFinished();
            }
        }
        else
        {
            if (am.Workplace.ReserveJob(am.Character) == false)
            {
                am.Character.WorkFinished();
            }
        }        
    }

    public override IAccessible GetDestinationTileOwner(BT_AgentMemory am)
    {
        return am.Workplace;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        return am.Workplace.GetAccessTile(am.UseWorkplaceSecondAccessTile);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        return am.Workplace.GetAccessTileRotation(am.UseWorkplaceSecondAccessTile);
    }
}
