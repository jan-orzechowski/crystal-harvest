using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToSourceStorage : BT_GoTo 
{
    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        if (am.Character.Reservation == null) { return null; }
        return am.Character.Reservation.SourceStorage.GetAccessTile();
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        if (am.Character.Reservation == null) { return Rotation.N; }
        return am.Character.Reservation.SourceStorage.GetAccessTileRotation();
    }
}
