using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToService : BT_GoTo
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Service != null);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        if (am.Service.ReserveService(am.Character) == false)
        {
            am.Character.ServiceEnded();
        }
      
        // am.Character.WorkFinished();
    }

    public override IAccessible GetDestinationTileOwner(BT_AgentMemory am)
    {
        return am.Service;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        return am.Service.GetAccessTile(am.UseServiceSecondAccessTile);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        return am.Service.GetAccessTileRotation(am.UseServiceSecondAccessTile);
    }
}
