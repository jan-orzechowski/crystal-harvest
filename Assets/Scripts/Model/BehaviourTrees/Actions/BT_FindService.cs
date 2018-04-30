using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindService : BT_Node 
{
    string need;

    public BT_FindService(string need) : base()
    {
        this.need = need;
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {        
        if (am.Service != null
            && am.Character.IsTileMarkedAsInaccessible(
               am.Service.GetAccessTile(am.UseServiceSecondAccessTile)) == false)
        {
            Debug.Log("mamy już dostępną usługę");
            return BT_Result.SUCCESS;
        }

        if (am.PotentialService == null)
        {
            am.Service = null;

            am.PotentialService = GameManager.Instance.World.GetClosestAvailableService(need, am.Character);

            if (am.PotentialService == null) return BT_Result.FAILURE;

            am.Character.SetNewDestination(am.PotentialService.GetAccessTile(), false);
            
            return BT_Result.RUNNING;
        }
        else
        {
            if (am.Character.Path == null 
                || am.Character.Path.Goal != am.PotentialService.GetAccessTile())
            {
                am.PotentialService = null;
                return BT_Result.RUNNING;
            }

            if (am.Character.Path.IsReady)
            {
                if (am.Character.Path.IsImpossible)
                {
                    am.PotentialService = null;
                    return BT_Result.RUNNING;
                }
                else
                {
                    am.Service = am.PotentialService;
                    am.PotentialService = null;
                    return BT_Result.SUCCESS;
                }
            }
            else
            {
                return BT_Result.RUNNING;
            }           
        }       
    }
}
