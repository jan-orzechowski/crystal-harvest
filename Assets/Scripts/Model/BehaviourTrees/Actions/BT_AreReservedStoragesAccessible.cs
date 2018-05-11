using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_AreReservedStoragesAccessible : BT_Node 
{
    public override bool IsAction { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        ResourceReservation reservation = am.Character.Reservation;

        if (reservation == null) return BT_Result.SUCCESS;

        if ((reservation.SourceStorage != null && am.Character.IsTileMarkedAsInaccessible(
                reservation.SourceStorage.GetAccessTile(reservation.UseSourceStorageSecondAccessTile)))
            || (am.Character.IsTileMarkedAsInaccessible(
                reservation.TargetStorage.GetAccessTile(reservation.UseTargetStorageSecondAccessTile))))
        {
            return BT_Result.FAILURE;            
        }
        else
        {
            return BT_Result.SUCCESS;
        }
    }
}
