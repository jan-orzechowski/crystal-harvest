using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToTargetStorage : BT_GoTo 
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Character.Reservation != null 
                && am.Character.HasResource);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        return;
    }

    public override IAccessible GetDestinationTileOwner(BT_AgentMemory am)
    {
        return am.Character.Reservation.TargetStorage;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        bool useSecondAccessTile = am.Character.Reservation.UseTargetStorageSecondAccessTile;
        return am.Character.Reservation.TargetStorage.GetAccessTile(useSecondAccessTile);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        bool useSecondAccessTile = am.Character.Reservation.UseTargetStorageSecondAccessTile;
        return am.Character.Reservation.TargetStorage.GetAccessTileRotation(useSecondAccessTile);
    }
}
