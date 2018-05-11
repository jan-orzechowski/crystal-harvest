using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToSourceStorage : BT_GoTo 
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Character.Reservation != null
                && am.Character.Reservation.SourceStorage != null
                && am.Character.HasResource == false);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        return;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        bool useSecondAccessTile = am.Character.Reservation.UseSourceStorageSecondAccessTile;
        return am.Character.Reservation.SourceStorage.GetAccessTile(useSecondAccessTile);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        bool useSecondAccessTile = am.Character.Reservation.UseSourceStorageSecondAccessTile;
        return am.Character.Reservation.SourceStorage.GetAccessTileRotation(useSecondAccessTile);
    }
}
