using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class StaticData
{
    public const float TimeLimit = (60f * 20f);

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
   
    public static Dictionary<string, string> LoadStrings()
    {
        return new Dictionary<string, string>()
        {
            // Wiadomości
            {"s_start_text", "Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy Długi tekst początkowy" },
            {"s_victory_text", "Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo Zwycięstwo " },
            {"s_defeat_text", "Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka Porażka " },
            {"s_start_confirmation", "Początek" },
            {"s_victory_confirmation", "Zwycięstwo" },
            {"s_defeat_confirmation", "Porażka" },

            // Zasoby
            {"s_crystals", "Kryształy" },
            {"s_metal", "Metal" },
            {"s_gas", "Gaz" },
            {"s_ore", "Ruda" },
            {"s_plants", "Rośliny" },
            {"s_medicine", "Leki" },
            {"s_food", "Żywność" },
            {"s_parts", "Części zamienne" },
            
            // Potrzeby
            {"Hunger", "Głód" },
            {"Health", "Zdrowie" },
            {"Condition", "Stan" },            

            // Budynki
            {"Spaceship", "Statek kosmiczny" },
            {"OreDeposit", "Ruda" },
            {"CrystalsDeposit", "Kryształy" },
            {"Stairs", "Schody" },
            {"Platform", "Platforma" },
            {"Slab", "Chodnik" },
            {"Greenhouse", "Szklarnia" },
            {"Storage", "Magazyn" },
            {"HealingChamber", "Komora lecznicza" },
            {"Quarters", "Kwatery" },
            {"Metalworks", "Huta" },
            {"Refinery", "Rafineria" },
            {"Laboratory", "Laboratorium" },
            {"FoodSynthesizer", "Syntezator żywności" },
            {"RobotFactory", "Fabryka robotów" },
            {"PartsFactory", "Fabryka części zamiennych" },
            {"RepairStation", "Stacja napraw" },
            {"CrystalStorage", "Magazyn na kryształy" },            

            // Opisy budynków
            {"SpaceshipDesc", "" },
            {"OreDepositDesc", "" },
            {"CrystalsDepositDesc", "" },
            {"StairsDesc", "Pozwalają na wchodzenie na platformy i skały." },
            {"PlatformDesc", "Zapewnia więcej przestrzeni na stawianie budynków." },
            {"SlabDesc", "Pozwala na szybsze poruszanie się." },
            {"GreenhouseDesc", "Tutaj hodowane są rośliny potrzebne do produkcji żywności oraz leków." },
            {"StorageDesc", "Pozwala przechowywać dowolne zasoby poza kryształami." },
            {"HealingChamberDesc", "Pozwala na wyleczenie kontuzji, jakich mogą nabawić się kosmonauci podczas pracy." },
            {"QuartersDesc", "Tutaj kosmonauci mogą spożyć posiłek i odpocząć." },
            {"MetalworksDesc", "Przetapia rudę na metal." },
            {"RefineryDesc", "W tym budynku wydobywany jest i przygotowywany do użycia gaz." },
            {"LaboratoryDesc", "Służy do wytwarzania leków dla kosmonautów." },
            {"FoodSynthesizerDesc", "Syntezator produkuje żywność z roślin." },
            {"RobotFactoryDesc", "Produkuje roboty pomagające kosmonautom w pracy." },
            {"PartsFactoryDesc", "Produkuje części potrzebne do konstrukcji i naprawy robotów." },
            {"RepairStationDesc", "Służy do naprawy uszkodzonych robotów." },
            {"CrystalStorageDesc", "Tylko w tym magazynie można przechowywać kryształy." },

            // Budowa
            {"s_halted", "Wstrzymane" },
            {"s_scaffolding_construction", "Budowa rusztowania" },
            {"s_scaffolding_deconstruction", "Rozbiórka rusztowania" },
            {"s_construction_site", "Plac budowy" },
            {"s_deconstruction_site", "Rozbiórka" },

            {"s_cancel_deconstruction_button", "Przywróć budynek" },
            {"s_deconstruction_button", "Dekonstrukcja" },
            {"s_halt_button", "Wstrzymaj działanie" },
            {"s_start_button", "Wznów działanie" },
            {"s_deconstruction_prompt", "Czy na pewno chcesz wyburzyć ten budynek?" },

            // Inne
            {"s_stats_button", "Statystki" },
            {"s_build_button", "Konstrukcja" },
            {"s_timer_hover", "Czas pozostały do zakończenia misji" },
            {"s_counter_hover", "Kryształy do zebrania" },
            {"s_robots_hover", "Roboty" },
            {"s_humans_hover", "Załoga" },
            {"s_yes", "Tak" },
            {"s_no", "Nie" },
        };
    }

    public static Dictionary<TileType, float> LoadTilesMovementCosts()
    {
        return new Dictionary<TileType, float>(){ {TileType.Empty, 0f},
                                                  {TileType.WalkableEmpty, 1f},
                                                  {TileType.Sand, 3f},
                                                  {TileType.Rock, 1f} };
    }

    public static Dictionary<int, ResourceInfo> LoadResources()
    {
        Dictionary<int, ResourceInfo> resources = new Dictionary<int, ResourceInfo>();

        resources.Add(0, new ResourceInfo() { Name = "s_crystals" });
        resources.Add(1, new ResourceInfo() { Name = "s_metal" });
        resources.Add(2, new ResourceInfo() { Name = "s_gas" });
        resources.Add(3, new ResourceInfo() { Name = "s_ore" });
        resources.Add(4, new ResourceInfo() { Name = "s_plants" });
        resources.Add(5, new ResourceInfo() { Name = "s_medicine" });
        resources.Add(6, new ResourceInfo() { Name = "s_food" });
        resources.Add(7, new ResourceInfo() { Name = "s_parts" });
        resources.Add(8, new ResourceInfo() { Name = "s_brain" });

        return resources;
    }

    public static void LoadNeeds(Character character)
    {
        if (character.IsRobot)
        {
            character.Needs = new Dictionary<string, float>()
            { {"Condition", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Condition", 0.02f} };
            // { { "Condition", 0.002f} };
        }
        else
        {
            character.Needs = new Dictionary<string, float>()
            { {"Health", 0f}, {"Hunger", 0f} };
            character.NeedGrowthPerSecond = new Dictionary<string, float>()
            { {"Health", 0.002f}, {"Hunger", 0.005f} };
            // { { "Health", 0.001f}, { "Hunger", 0.003f} };
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
        bp.DescriptionKey = "SpaceshipDesc";

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

        bp.MaxStorage = 24;
        bp.InitialStorage = new Dictionary<int, int>() { { 1, 12 },
                                                         { 5, 6 },
                                                         { 7, 6 } };

        bp.CanBeDeconstructed = false;

        prototypes.Add(bp);


        // -----------------------------------
        // ORE DEPOSIT
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "OreDeposit";
        bp.DescriptionKey = "OreDepositDesc";

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
        bp.DescriptionKey = "CrystalsDepositDesc";

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
        bp.DescriptionKey = "StairsDesc";

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
       
        prototypes.Add(bp);


        // -----------------------------------
        // PLATFORM
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Platform";
        bp.DescriptionKey = "PlatformDesc";

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

        bp.CanBeDeconstructed = false;

        prototypes.Add(bp);


        // -----------------------------------
        // SLAB
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Slab";
        bp.DescriptionKey = "SlabDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // GREENHOUSE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Greenhouse";
        bp.DescriptionKey = "GreenhouseDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Storage";
        bp.DescriptionKey = "StorageDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // HEALING CHAMBER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "HealingChamber";
        bp.DescriptionKey = "HealingChamberDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // QUARTERS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Quarters";
        bp.DescriptionKey = "QuartersDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // METALWORKS
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Metalworks";
        bp.DescriptionKey = "MetalworksDesc";

        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnPlatform = true;

        bp.NormalizedTilePositions = BuildingPrototype.GetNormalizedTilePositions(1, 2);

        bp.HasAccessTile = true;
        bp.NormalizedAccessTilePosition = new TilePosition(0, 2, 0);
        bp.NormalizedAccessTileRotation = Rotation.S;

        bp.StartingRotation = Rotation.N;

        bp.ConstructionTime = 5f;
        bp.ConstructionResources = new Dictionary<int, int>() { { 1, 2 } };
        bp.ResourcesFromDeconstruction = new Dictionary<int, int>() { { 1, 1 } };

        bp.ProductionTime = 10f;
        bp.ConsumedResources = new Dictionary<int, int>() { { 3, 1 } };
        bp.ProducedResources = new Dictionary<int, int>() { { 1, 2 } };

        prototypes.Add(bp);


        // -----------------------------------
        // REFINERY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Refinery";
        bp.DescriptionKey = "RefineryDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // LABORATORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "Laboratory";
        bp.DescriptionKey = "LaboratoryDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // FOOD SYNTHESIZER
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "FoodSynthesizer";
        bp.DescriptionKey = "FoodSynthesizerDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // ROBOT FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RobotFactory";
        bp.DescriptionKey = "RobotFactoryDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // PARTS FACTORY
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "PartsFactory";
        bp.DescriptionKey = "PartsFactoryDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // REPAIR STATION
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "RepairStation";
        bp.DescriptionKey = "RepairStationDesc";

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

        prototypes.Add(bp);


        // -----------------------------------
        // CRYSTAL STORAGE
        // -----------------------------------

        bp = new BuildingPrototype();
        bp.Type = "CrystalStorage";
        bp.DescriptionKey = "CrystalStorageDesc";

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

        prototypes.Add(bp);

        return prototypes;
    }
}
