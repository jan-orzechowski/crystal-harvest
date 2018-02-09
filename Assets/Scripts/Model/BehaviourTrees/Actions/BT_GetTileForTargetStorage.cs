using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTileForTargetStorage : BT_Node 
{
    string tileVarName;
    public BT_GetTileForTargetStorage(string tileVarName) : base()
    {
        this.tileVarName = tileVarName;
    }
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Reservation == null) { return BT_Result.FAILURE; }
        if (am.Reservation.TargetStorage == null) { return BT_Result.ERROR; }
        Tile tile = am.Reservation.TargetStorage.GetAccessTile();
        am.SetGlobalTile(tileVarName, tile);
        return BT_Result.SUCCESS;
    }
}
