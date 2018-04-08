using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IWorkplace : IBuildingModule
{
    bool Work(float deltaTime, Character character);
    bool Halted { get; }
    void SetHaltStatus(bool halt);
    bool CanReserveJob(Character character);
    bool ReserveJob(Character character);
    void RenewJobReservation(Character character);
    StorageToFill InputStorage { get; }
    StorageToEmpty OutputStorage { get; }
    Tile GetAccessTile();
    Rotation GetAccessTileRotation();
}
