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
        if (am.Reservation != null) { return BT_Result.FAILURE; }

        Service service = GameManager.Instance.World.GetClosestService(need, am.Character);

        if (service == null)
        {
            return BT_Result.FAILURE;
        }
       
        ResourceReservation newReservation
            = GameManager.Instance.World.GetReservationForFillingInput(am.Character, service.InputStorage);

        if (newReservation == null)
        {
            return BT_Result.FAILURE;
        }
        else
        {
            if (service.ReserveService(newReservation.Resource, am.Character))
            {
                am.SetNewService(service);
                am.SetNewReservation(newReservation);
                Debug.Log("Udana rezerwacja!");
                return BT_Result.SUCCESS;
            }
            else
            {
                return BT_Result.FAILURE;
            }

        }
    }
}
