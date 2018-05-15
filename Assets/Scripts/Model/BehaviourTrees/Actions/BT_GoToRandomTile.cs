using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToRandomTile : BT_GoTo
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.RandomTile != null);
    }

    public override IAccessible GetDestinationTileOwner(BT_AgentMemory am)
    {
        return null;
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        return;
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        return am.RandomTile;
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        return am.RandomTileRotation;
    }

    public override void Deactivate(BT_AgentMemory am)
    {
        base.Deactivate(am);
        am.RandomTile = null;
    }
}
