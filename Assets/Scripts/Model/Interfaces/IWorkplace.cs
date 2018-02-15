using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//public enum WorkResult
//{
//    Success,
//    Running,
//    Failure
//}

public interface IWorkplace : IStorage
{
    bool Work(float deltaTime, Character character);
    bool IsJobFree();
    bool ReserveJob();
    bool CanReserveResource(int resourceID, Character character);
    bool CanReserveFreeSpace(int resourceID, Character character);
    int MissingResourcesCount { get; }
    int ResourcesToRemoveCount { get; }
    Dictionary<int, int> MissingResources { get; }
    Dictionary<int, int> OutputResources { get; }
}
