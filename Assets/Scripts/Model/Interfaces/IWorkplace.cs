using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IWorkplace
{
    bool Work(float deltaTime, Character character);
    bool IsJobFree();
    bool ReserveJob(Character character);
    void RenewJobReservation(Character character);
    StorageToFill InputStorage { get; }
    StorageToEmpty OutputStorage { get; }
    Tile GetAccessTile();
    Rotation GetAccessTileRotation();
}
