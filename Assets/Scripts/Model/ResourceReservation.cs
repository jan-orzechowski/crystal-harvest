using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceReservation
{
    public Storage SourceStorage;
    public Storage TargetStorage;

    public bool UseSourceStorageSecondAccessTile;
    public bool UseTargetStorageSecondAccessTile;

    public int Resource;

    public ResourceReservation(Storage sourceStorage, bool useSourceStorageSecondAccessTile,
                               Storage targetStorage, bool useTargetStorageSecondAccessTile,
                               int resourceID)
    {
        SourceStorage = sourceStorage;
        TargetStorage = targetStorage;

        UseSourceStorageSecondAccessTile = useSourceStorageSecondAccessTile;
        UseTargetStorageSecondAccessTile = useTargetStorageSecondAccessTile;

        Resource = resourceID;
    }
}
