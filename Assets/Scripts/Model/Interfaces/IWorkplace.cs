using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IWorkplace : IBuildingModule, IAccessible
{
    bool Work(float deltaTime, Character character);
    bool CanReserveJob(Character character);
    bool ReserveJob(Character character);
    StorageWithRequirements InputStorage { get; }
    Storage OutputStorage { get; }
}