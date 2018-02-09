using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceReservation
{
    public IStorage SourceStorage;
    public IStorage TargetStorage;
    public int Resource;

    public ResourceReservation(IStorage sourceStorage, IStorage targetStorage, int resourceID)
    {
        SourceStorage = sourceStorage;
        TargetStorage = targetStorage;
        Resource = resourceID;
    }
}
