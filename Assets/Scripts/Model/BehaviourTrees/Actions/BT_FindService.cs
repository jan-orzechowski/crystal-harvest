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
        if (am.Character.Reservation != null) { return BT_Result.FAILURE; }

        Service service = GameManager.Instance.World.GetClosestService(need, am.Character);

        if (service == null)
        {
            return BT_Result.FAILURE;
        }

        if (service.CanReserveService(am.Character)
            && GameManager.Instance.World.GetReservationForFillingInput(am.Character, service.InputStorage))
        {
            if (service.ReserveService(am.Character))
            {
                am.SetNewService(service);
                return BT_Result.SUCCESS;
            }
            else
            {
                return BT_Result.FAILURE;
            }
        }
        else
        {            
            return BT_Result.FAILURE;
        }
    }
}
