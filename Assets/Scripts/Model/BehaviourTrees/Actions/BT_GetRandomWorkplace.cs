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

        for (int i = 0; i < 5; i++)
        {
            int f = UnityEngine.Random.Range(0, world.Factories.Count);
            if (world.Factories[f].IsJobFree())
            {
                am.SetNewWorkplace(world.Factories[f]);
                //Debug.Log("Miejsce pracy znalezione: " + world.Factories[f].Building.Type);
                return BT_Result.SUCCESS;
            }
        }

        return BT_Result.FAILURE;
    }
}
