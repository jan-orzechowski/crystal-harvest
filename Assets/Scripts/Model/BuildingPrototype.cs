using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingPrototype
{    
    public string Type;
    public string ModelName;

    public bool CanBeBuiltOnSand; 
    public bool CanBeBuiltOnRock;
    public bool CanBeBuiltOnPlatform;

    public bool AllowRotation;
    public List<TilePosition> NormalizedTilePositions;

    public TilePosition MousePivotPoint;
    public Rotation StartingRotation;

    public bool HasAccessTile;
    public TilePosition NormalizedAccessTilePosition;
    public Rotation NormalizedAccessTileRotation;

    public bool HasSecondAccessTile;
    public TilePosition NormalizedSecondAccessTilePosition;
    public Rotation NormalizedSecondAccessTileRotation;

    public float MovementCost;

    public bool WalkableOnTop;
    public float MovementCostOnTop;

    public int MaxStorage;
    public Dictionary<int, int> InitialStorage;

    public float ProductionTime;
    public Dictionary<int, int> ConsumedResources;
    public Dictionary<int, int> ProducedResources;

    public float ConstructionTime;
    public Dictionary<int, int> ConstructionResources;
    public Dictionary<int, int> ResourcesFromDeconstruction;

    public string NeedFulfilled;
    public float NeedFulfillmentPerSecond;
    public float ServiceDuration;
}
