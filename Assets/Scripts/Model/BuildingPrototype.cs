using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingPrototype
{
    public string Type;

    public bool CanBeBuiltOnSand = true;
    public bool CanBeBuiltOnRock = false;
    public bool CanBeBuiltOnPlatform = false;

    public bool MultipleBuildMode = false;

    public bool AllowRotation = true;
    public List<TilePosition> NormalizedTilePositions;

    public TilePosition MousePivotPoint = new TilePosition(0, 0, 0);
    public Rotation StartingRotation = Rotation.N;

    public bool HasAccessTile = false;
    public TilePosition NormalizedAccessTilePosition;
    public Rotation NormalizedAccessTileRotation;

    public bool HasSecondAccessTile = false;
    public TilePosition NormalizedSecondAccessTilePosition;
    public Rotation NormalizedSecondAccessTileRotation;

    public float MovementCost = 0f;
    public bool AllowToBuildOnTop = false;
    public bool DisallowDiagonalMovement = false;

    public bool WalkableOnTop = false;
    public bool CanBeAccessedFromTop = false;
    public float MovementCostOnTop = 0f;

    public int MaxStorage = 0;
    public Dictionary<int, int> InitialStorage = null;
    public List<int> RestrictedResources = new List<int>() { 0, 3 };

    public float ProductionTime = 0f;
    public Dictionary<int, int> ConsumedResources = null;
    public Dictionary<int, int> ProducedResources = null;

    public int ProductionCyclesLimitMin = -1;
    public int ProductionCyclesLimitMax = -1;

    public bool ProducesRobot = false;
    public bool IsNaturalDeposit = false;

    public float ConstructionTime = 0f;
    public Dictionary<int, int> ConstructionResources = null;
    public Dictionary<int, int> ResourcesFromDeconstruction = null;
    public bool ConstructionWithoutScaffolding = false;
    public float ConstructionSiteModelHeightOffset = 0f;

    public bool CanBeDeconstructed = true;

    public Dictionary<int, int> ResourcesForScaffoldingConstruction = new Dictionary<int, int>() { { 1, 1 } };

    public Dictionary<string, float> NeedGrowthPerSecond = null;

    public string NeedFulfilled = null;
    public float NeedFulfillmentPerSecond;
    public float ServiceDuration;

    public bool HidesCharacter = false;
    
    public string DescriptionKey = "";

    public static List<TilePosition> GetNormalizedTilePositions(int xSize, int ySize)
    {
        List<TilePosition> result = new List<TilePosition>();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                result.Add(new TilePosition(x, y, 0));
            }
        }
        return result;
    }
}
