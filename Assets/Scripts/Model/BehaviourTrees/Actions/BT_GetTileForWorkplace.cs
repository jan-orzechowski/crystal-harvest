using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetTileForWorkplace : BT_Node 
{
    string tileVarName;
    public BT_GetTileForWorkplace(string tileVarName) : base()
    {
        this.tileVarName = tileVarName;
    }
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null) { return BT_Result.ERROR; }

        Tile tile = am.Workplace.GetAccessTile();    
        am.SetGlobalTile(tileVarName, tile);
        return BT_Result.SUCCESS;
    }
}
