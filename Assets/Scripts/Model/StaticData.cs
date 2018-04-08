using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticData
{
    public const int WorldWidth = 50;
    public const int WorldLenght = 50;

    public const int MinOreAmountOnMap = 100;
    public const int MaxOreAmountOnMap = 200;

    public const int MinOreAmountInDeposit = 4;
    public const int MaxOreAmountInDeposit = 10;

    public const int MinCrystalsAmountOnMap = 100;
    public const int MaxCrystalsAmountOnMap = 200;

    public const int MinCrystalsAmountInDeposit = 1;
    public const int MaxCrystalsAmountInDeposit = 15;

    public const int CrystalsAmountToGather = 100;

    public static Dictionary<int, ResourceInfo> LoadResources()
    {
        Dictionary<int, ResourceInfo> resources = new Dictionary<int, ResourceInfo>();

        resources.Add(0, new ResourceInfo() { Name = "Kryształy" });
        resources.Add(1, new ResourceInfo() { Name = "Metal" });
        resources.Add(2, new ResourceInfo() { Name = "Gaz" });
        resources.Add(3, new ResourceInfo() { Name = "Ruda" });
        resources.Add(4, new ResourceInfo() { Name = "Rośliny" });
        resources.Add(5, new ResourceInfo() { Name = "Leki" });
        resources.Add(6, new ResourceInfo() { Name = "Żywność" });
        resources.Add(7, new ResourceInfo() { Name = "Części zamienne" });
        resources.Add(8, new ResourceInfo() { Name = "Mózg elektronowy" });

        return resources;
    }

    public static void LoadNeeds(Character character)
    {
        if (character.IsRobot)
        {
            character.Needs = new Dictionary<string, float>()
            { {"Condition", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Condition", 0.05f} };
        }
        else
        {
            character.Needs = new Dictionary<string, float>()
            { {"Health", 0f}, {"Hunger", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Health", 0.05f}, {"Hunger", 0.05f} };
        }
    }

    public static List<BuildingPrototype> LoadPrototypes()
    {
        List<BuildingPrototype> prototypes = new List<BuildingPrototype>(24);
        BuildingPrototype bp;

        // -----------------------------------
        // SPACESHIP
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Spaceship";

        bp.CanBeBuiltOnRock = true;

        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(0,0,0),                          new TilePosition(2,0,0),
            new TilePosition(0,1,0), new TilePosition(1,1,0), new TilePosition(2,1,0),
            new TilePosition(0,2,0), new TilePosition(1,2,0), new TilePosition(2,2,0),
            new TilePosition(0,3,0), new TilePosition(1,3,0), new TilePosition(2,3,0),
            new TilePosition(0,4,0), new TilePosition(1,4,0), new TilePosition(2,4,0),
        };

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 0, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.MaxStorage = 90;
        bp.InitialStorage = new Dictionary<int, int>() { { 1, 10 }, { 2, 10 }, { 3, 10 },
                                                         { 4, 10 }, { 5, 10 }, { 6, 10 },
                                                         { 7, 10 }, { 8, 10 }, { 0, 10 }};

        prototypes.Add(bp);


        // -----------------------------------
        // ORE DEPOSIT
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "OreDeposit";

        bp.CanBeBuiltOnRock = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionWithoutScaffolding = true;

        bp.IsNaturalDeposit = true;

        bp.ProductionTime = 2f;
        bp.ProducedResources = new Dictionary<int, int>() { { 3, 1 } };
        bp.ProductionCyclesLimitMin = MinOreAmountInDeposit;
        bp.ProductionCyclesLimitMax = MaxOreAmountInDeposit;

        prototypes.Add(bp);


        // -----------------------------------
        // CRYSTALS DEPOSIT
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "CrystalsDeposit";
       
        bp.CanBeBuiltOnRock = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionWithoutScaffolding = true;

        bp.IsNaturalDeposit = true;

        bp.ProductionTime = 2f;
        bp.ProducedResources = new Dictionary<int, int>() { { 0, 1 } };
        bp.ProductionCyclesLimitMin = MinCrystalsAmountInDeposit;
        bp.ProductionCyclesLimitMax = MaxCrystalsAmountInDeposit;

        prototypes.Add(bp);


        // -----------------------------------
        // STAIRS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Stairs";

        bp.CanBeBuiltOnSand = true;

        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(1,0,0), new TilePosition(2,0,0)
        };

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 0, 1);
        bp.NormalizedAccessTileRotation = Rotation.E;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(3, 0, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.W;

        bp.ConstructionTime = 3f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;

        bp.MousePivotPoint = new TilePosition(1, 0, 0);

        bp.Description = "Pozwalają na wchodzenie na platformy i skały.";

        prototypes.Add(bp);


        // -----------------------------------
        // PLATFORM
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Platform";
        
        bp.CanBeBuiltOnSand = true;

        bp.MultipleBuildMode = true;

        bp.AllowRotation = false;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.MovementCost = 2f;
        bp.WalkableOnTop = true;
        bp.CanBeAccessedFromTop = true;
        bp.DisallowDiagonalMovement = true;
        bp.MovementCostOnTop = 2f;

        bp.ConstructionTime = 3f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;
        
        bp.Description = "Zapewnia więcej przestrzeni na stawianie budynków.";

        prototypes.Add(bp);


        // -----------------------------------
        // SLAB
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Slab";

        bp.CanBeBuiltOnSand = true;
        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.MultipleBuildMode = true;

        bp.AllowRotation = false;

        bp.MovementCost = 0.5f;
        bp.AllowToBuildOnTop = true;
        bp.HasAccessTile = false;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.ConstructionTime = 3f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 1 } };
        bp.ConstructionWithoutScaffolding = true;

        bp.Description = "Pozwala na szybsze poruszanie się.";

        prototypes.Add(bp);


        // -----------------------------------
        // GREENHOUSE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Greenhouse";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ProductionTime = 5f;
        bp.ProducedResources = new Dictionary<int, int>() { { 4, 2 } };

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.HidesCharacter = true;

        bp.Description = "Tutaj hodowane są rośliny potrzebne do produkcji żywności oraz leków.";

        prototypes.Add(bp);


        // -----------------------------------
        // STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Storage";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.MaxStorage = 18;

        bp.Description = "Pozwala przechowywać dowolne zasoby poza kryształami.";

        prototypes.Add(bp);


        // -----------------------------------
        // HEALING CHAMBER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "HealingChamber";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(1, 2, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.S;
        
        bp.ConsumedResources = new Dictionary<int, int>() { { 5, 1 } };
        bp.NeedFulfilled = "Health";
        bp.NeedFulfillmentPerSecond = 1f;
        bp.ServiceDuration = 5f;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.MousePivotPoint = new TilePosition(1, 1, 0);
        bp.StartingRotation = Rotation.S;

        bp.HidesCharacter = true;

        bp.Description = "Pozwala na wyleczenie kontuzji, jakich mogą nabawić się kosmonauci podczas pracy.";

        prototypes.Add(bp);


        // -----------------------------------
        // QUARTERS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Quarters";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(3, 3);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ConsumedResources = new Dictionary<int, int>() { { 6, 1 } };
        bp.NeedFulfilled = "Hunger";
        bp.NeedFulfillmentPerSecond = 1f;
        bp.ServiceDuration = 5f;

        bp.HidesCharacter = true;

        bp.Description = "Tutaj kosmonauci mogą spożyć posiłek i odpocząć.";

        prototypes.Add(bp);


        // -----------------------------------
        // METALWORKS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Metalworks";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.N;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 5f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 3, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 1, 2 } };

        bp.Description = "Przetapia rudę na metal.";

        prototypes.Add(bp);


        // -----------------------------------
        // REFINERY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Refinery";

        bp.CanBeBuiltOnSand = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(2, 1, 0);
        bp.NormalizedAccessTileRotation = Rotation.W;

        bp.StartingRotation = Rotation.E;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 10f;
        bp.ProducedResources = new Dictionary<int, int>() { { 2, 3 } };

        bp.Description = "W tym budynku wydobywany jest i przygotowywany do użycia gaz.";

        prototypes.Add(bp);


        // -----------------------------------
        // LABORATORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Laboratory";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.S;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 5f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 2, 1 }, { 4, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 5, 1 } };

        bp.Description = "Służy do wytwarzania leków dla kosmonautów.";

        prototypes.Add(bp);


        // -----------------------------------
        // FOOD SYNTHESIZER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "FoodSynthesizer";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 5f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 4, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 6, 1 } };

        bp.Description = "Syntezator produkuje żywność z roślin.";

        prototypes.Add(bp);


        // -----------------------------------
        // ROBOT FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RobotFactory";
        
        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;
        
        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(3, 3);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 3, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;
        bp.HasSecondAccessTile = true;
        bp.NormalizedSecondAccessTilePosition = new TilePosition(1, -1, 0);
        bp.NormalizedSecondAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.W;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 10f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 1, 1 }, { 2, 1 }, { 7, 2 } };        
        bp.ProducesRobot = true;

        bp.Description = "Produkuje roboty pomagające kosmonautom w pracy."; 

        prototypes.Add(bp);


        // -----------------------------------
        // PARTS FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "PartsFactory";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(2, 4);

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(-1, 0, 0);
        bp.NormalizedAccessTileRotation = Rotation.E;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ProductionTime = 5f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 1, 1 }, { 2, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 7, 1 } };

        bp.Description = "Produkuje części potrzebne do konstrukcji i naprawy robotów.";

        prototypes.Add(bp);


        // -----------------------------------
        // REPAIR STATION
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RepairStation";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.StartingRotation = Rotation.E;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.ConsumedResources = new Dictionary<int, int>() { { 7, 1 } };
        bp.NeedFulfilled = "Condition";
        bp.NeedFulfillmentPerSecond = 1f;
        bp.ServiceDuration = 5f;

        bp.HidesCharacter = true;

        bp.Description = "Służy do naprawy uszkodzonych robotów.";

        prototypes.Add(bp);


        // -----------------------------------
        // CRYSTAL STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "CrystalStorage";

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 1);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, -1, 0);
        bp.NormalizedAccessTileRotation = Rotation.N;

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 3 }, { 7, 1 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 2 } };

        bp.MaxStorage = 12;
        bp.RestrictedResources = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

        bp.Description = "Tylko w tym magazynie można przechowywać kryształy.";

        prototypes.Add(bp);

        return prototypes;
    }
}
