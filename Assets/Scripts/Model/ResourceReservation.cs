using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceReservation
{
    public ISourceStorage SourceStorage;
    public ITargetStorage TargetStorage;
    public int Resource;

    public ResourceReservation(ISourceStorage sourceStorage, ITargetStorage targetStorage, int resourceID)
    {
        SourceStorage = sourceStorage;
        TargetStorage = targetStorage;
        Resource = resourceID;
    }
}
