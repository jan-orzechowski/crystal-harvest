using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetRandomWorkplace : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        World world = GameManager.Instance.World;
        
        for (int i = 0; i < 5; i++)
        {
            if (world.ConstructionSites.Count == 0) break;

            int cs = UnityEngine.Random.Range(0, world.ConstructionSites.Count);
            if (world.ConstructionSites[cs].CanReserveJob(am.Character))
            {
                am.SetNewWorkplace((IWorkplace)world.ConstructionSites[cs]);
                return BT_Result.SUCCESS;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            if (world.Factories.Count == 0) break;

            int f = UnityEngine.Random.Range(0, world.Factories.Count);
            if (world.Factories[f].CanReserveJob(am.Character))
            {
                am.SetNewWorkplace((IWorkplace)world.Factories[f]);
                return BT_Result.SUCCESS;
            }
        }

        return BT_Result.FAILURE;
    }
}
