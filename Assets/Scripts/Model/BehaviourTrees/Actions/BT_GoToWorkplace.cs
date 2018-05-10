using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToWorkplace : BT_GoTo
{
    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Workplace != null);
    }

    public override void WhileRunning(BT_AgentMemory am)
    {
        if (am.Character.Reservation != null
            && am.Character.Reservation.TargetStorage == am.Workplace.InputStorage)
        {
            am.Workplace.RenewJobReservation(am.Character);
        }
    }

    public override Tile GetDestinationTile(BT_AgentMemory am)
    {
        return am.Workplace.GetAccessTile(am.UseWorkplaceSecondAccessTile);
    }

    public override Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        return am.Workplace.GetAccessTileRotation(am.UseWorkplaceSecondAccessTile);
    }
}
