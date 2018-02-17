using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTileForService : BT_Node 
{
    string tileVarName;
    public BT_GetTileForService(string tileVarName) : base()
    {
        this.tileVarName = tileVarName;
    }
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null) { return BT_Result.ERROR; }

        Tile tile = am.Service.GetAccessTile();
        am.SetGlobalTile(tileVarName, tile);
        return BT_Result.SUCCESS;
    }
}
