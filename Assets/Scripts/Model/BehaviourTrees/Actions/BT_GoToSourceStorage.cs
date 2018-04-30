using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToSourceStorage : BT_GoTo 
{
    public override bool CheckConditions(BT_AgentMemory am)
    {
        return (am.Character.Reservation != null
                && am.Character.HasResource == false);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        return;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        return am.Character.Reservation.SourceStorage.GetAccessTile(false);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        return am.Character.Reservation.SourceStorage.GetAccessTileRotation(false);
    }
}
