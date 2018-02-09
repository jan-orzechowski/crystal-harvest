using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetRandomWorkplace : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        World world = GameManager.Instance.World;

        if (world.Factories.Count == 0) return BT_Result.FAILURE;

        int i = UnityEngine.Random.Range(0, world.Factories.Count);
        am.SetNewWorkplace(world.Factories[i]);
        Debug.Log("Miejsce pracy znalezione: " + world.Factories[i].Building.Type);
        return BT_Result.SUCCESS;
    }
}
