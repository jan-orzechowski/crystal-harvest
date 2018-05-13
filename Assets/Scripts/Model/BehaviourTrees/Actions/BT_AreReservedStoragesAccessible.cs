using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_AreReservedStoragesAccessible : BT_Node 
{
    public override bool Activates { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        ResourceReservation reservationToCheck = am.Character.Reservation;

        if (reservationToCheck == null) return BT_Result.SUCCESS;

        if ((reservationToCheck.SourceStorage != null && am.Character.IsTileMarkedAsInaccessible(
                reservationToCheck.SourceStorage.GetAccessTile(reservationToCheck.UseSourceStorageSecondAccessTile)))
            || (am.Character.IsTileMarkedAsInaccessible(
                reservationToCheck.TargetStorage.GetAccessTile(reservationToCheck.UseTargetStorageSecondAccessTile))))
        {
            return BT_Result.FAILURE;            
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
