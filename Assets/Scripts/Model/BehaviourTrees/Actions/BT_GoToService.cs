using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToService : BT_GoTo 
{
    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        if (am.Service == null) { return null; }
        return am.Service.GetAccessTile();
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        if (am.Service == null) { return Rotation.N; }
        return am.Service.GetAccessTileRotation();
    }
}
