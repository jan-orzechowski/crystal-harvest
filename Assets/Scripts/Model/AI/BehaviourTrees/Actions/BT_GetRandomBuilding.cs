using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetRandomBuilding : BT_ActionNode
{
    string tileVarName;
    int maxSearchesNumber = 10;

    public BT_GetRandomBuilding(string tileVarName) : base()
    {
        this.tileVarName = tileVarName;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        World world = GameManager.Instance.World;

        if (world.Buildings.Count == 0) return BT_Result.FAILURE;

        for (int i = 0; i < maxSearchesNumber; i++)
        {
            int b = UnityEngine.Random.Range(0, world.Buildings.Count);
            if (world.Buildings[b] != null && world.Buildings[b].AccessTile != null)
            {
                am.SetGlobalTile(tileVarName, world.Buildings[b].AccessTile);
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;      
    }
}
