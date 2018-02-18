using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTileForSourceStorage : BT_Node
{
    string tileVarName;
    public BT_GetTileForSourceStorage(string tileVarName) : base()
    {
        this.tileVarName = tileVarName;
    }
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.Reservation == null) { return BT_Result.FAILURE; }
        if (am.Character.Reservation.SourceStorage == null) { return BT_Result.ERROR; }
        Tile tile = am.Character.Reservation.SourceStorage.GetAccessTile();
        am.SetGlobalTile(tileVarName, tile);
        return BT_Result.SUCCESS;
    }
}
