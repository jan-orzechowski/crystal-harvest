using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_Move : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.IsMoving())
        {
            if (am.Service != null
                && ((am.Character.Reservation != null && am.Character.Reservation.TargetStorage == am.Service.InputStorage)
                     || am.Service.GetAccessTile() == am.Character.DestinationTile))
            {
                am.Service.RenewServiceReservation(am.Character);
            }
            else if (am.Workplace != null 
                     && ((am.Character.Reservation != null && am.Character.Reservation.TargetStorage == am.Workplace.InputStorage)
                        || am.Workplace.GetAccessTile() == am.Character.DestinationTile))
            {
                am.Workplace.RenewJobReservation(am.Character);
            }

            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
