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

    public TilePosition MousePivotPoint = new TilePosition(0, 0, 0);
    public Rotation StartingRotation = Rotation.N;

    public bool HasAccessTile;
    public TilePosition NormalizedAccessTilePosition;
    public Rotation NormalizedAccessTileRotation;

    public bool HasSecondAccessTile;
    public TilePosition NormalizedSecondAccessTilePosition;
    public Rotation NormalizedSecondAccessTileRotation;

    public float MovementCost;
    public bool AllowToBuildOnTop;
    public bool DisallowDiagonalMovement;

    public bool WalkableOnTop;
    public bool CanBeAccessedFromTop;
    public float MovementCostOnTop;

    public int MaxStorage;
    public Dictionary<int, int> InitialStorage;

    public float ProductionTime;
    public Dictionary<int, int> ConsumedResources;
    public Dictionary<int, int> ProducedResources;

    public int ProductionCyclesLimitMin = -1;
    public int ProductionCyclesLimitMax = -1;

    public float ConstructionTime;
    public Dictionary<int, int> ConstructionResources;
    public Dictionary<int, int> ResourcesFromDeconstruction;
    public bool ConstructionWithoutScaffolding;

    public string NeedFulfilled;
    public float NeedFulfillmentPerSecond;
    public float ServiceDuration;
}
