using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToWorkplace : BT_GoTo 
{
    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        if (am.Workplace == null) { return null; }
        return am.Workplace.GetAccessTile();
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        if (am.Workplace == null) { return Rotation.N; }
        return am.Workplace.GetAccessTileRotation();
    }
}
