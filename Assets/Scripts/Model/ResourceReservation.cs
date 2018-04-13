using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceReservation
{
    public Storage SourceStorage;
    public Storage TargetStorage;
    public int Resource;

    public ResourceReservation(Storage sourceStorage, Storage targetStorage, int resourceID)
    {
        SourceStorage = sourceStorage;
        TargetStorage = targetStorage;
        Resource = resourceID;
    }
}
