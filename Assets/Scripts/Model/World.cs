using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Pathfinding;

public class World
{
    public int XSize { get; protected set; }
    public int YSize { get; protected set; }
    public int Height { get; protected set; }

    public Tile[,,] Tiles { get; protected set; }

    public Dictionary<TileType, float> TileTypesMovementCosts { get; protected set; }

    public List<Building> Buildings { get; protected set; }
    public List<Character> Characters { get; protected set; }

    List<BuildingPrototype> buildingPrototypes;
    public Dictionary<int, ResourceInfo> ResourcesInfo { get; protected set; }
    public List<Storage> Storages { get; protected set; }
    public List<Factory> Factories { get; protected set; }
    public List<Factory> NonDepositFactories { get; protected set; }
    public List<Factory> NaturalDeposits { get; protected set; }
    public List<ConstructionSite> ConstructionSites { get; protected set; }
    public Dictionary<string, List<Service>> Services { get; protected set; }
    public List<Building> Stairs { get; protected set; }
    public Dictionary<Tile, Building> Platforms { get; protected set; }

    HashSet<Tile> tilesWithPlatformConstructionSites;
    HashSet<Tile> tilesWithSlabConstructionSites;
    HashSet<Tile> tilesWithSlabs;

    List<Building> buildingsMarkedForDeconstruction;
    List<Character> charactersMarkedForDeletion;

    public Dictionary<int, int> AllResources { get; protected set; }

    public int ResourceToGather { get; protected set; }
    public int AmountToGather { get; protected set; }

    public Pathfinder Pathfinder { get; protected set; }

    BT_Tree humanBehaviourTree;
    BT_Tree robotBehaviourTree;

    List<Tile> modifiedTiles;
    bool mapChangedThisFrame;

    int humanCounter;
    int robotCounter;

    public int HumanNumber { get; protected set; }
    public int RobotNumber { get; protected set; }

    public int StartingAreaX { get; protected set; }
    public int StartingAreaY { get; protected set; }

    Dictionary<TilePosition, int> oreDeposits;
    Dictionary<TilePosition, int> crystalsDeposits;

    Dictionary<Character, float> crystalsDepositsReservations;
    Dictionary<Character, float> oreDepositsReservations;
    float naturalDepositsReservationTimer = 15f;

    int totalSpaceForCrystals;
    int totalSpaceForOre;

    public bool Paused;
    public bool CannotUnpause { get; protected set; }

    public bool initialTextShown;

    public float TimeLeft { get; protected set; }

    bool gameEnded;

    List<string> unusedCharacterNames;
    string[] letters;
    string[] digits;

    public World(int width, int length, int startingAreaXSize, int startingAreaYSize)
    {
        XSize = width;
        YSize = length;
        Height = 2;

        TileTypesMovementCosts = StaticData.LoadTilesMovementCosts();

        MapGenerator mapGenerator = new MapGenerator();
        Tiles = mapGenerator.GenerateMap(XSize, YSize, startingAreaXSize, startingAreaYSize);

        StartingAreaX = mapGenerator.StartingAreaX;
        StartingAreaY = mapGenerator.StartingAreaY;

        oreDeposits = mapGenerator.Ore;
        crystalsDeposits = mapGenerator.Crystals;

        Tile.SetWorldForTiles(this);

        Debug.Log("Stworzono mapę posiadającą " + XSize * YSize + " pól.");

        Buildings = new List<Building>();
        Characters = new List<Character>();

        buildingPrototypes = StaticData.LoadPrototypes();
        ResourcesInfo = StaticData.LoadResources();

        AllResources = new Dictionary<int, int>();
        foreach (int resourceID in ResourcesInfo.Keys)
        {
            AllResources.Add(resourceID, 0);
        }

        ResourceToGather = 0;
        AmountToGather = StaticData.CrystalsAmountToGather;

        Pathfinder = new Pathfinder(this);
        modifiedTiles = new List<Tile>();

        humanBehaviourTree = new BT_Tree();
        humanBehaviourTree.LoadHumanTree();

        robotBehaviourTree = new BT_Tree();
        robotBehaviourTree.LoadRobotTree();

        Storages = new List<Storage>();
        Factories = new List<Factory>();
        NonDepositFactories = new List<Factory>();
        NaturalDeposits = new List<Factory>();
        Services = new Dictionary<string, List<Service>>();
        Stairs = new List<Building>();
        Platforms = new Dictionary<Tile, Building>();

        buildingsMarkedForDeconstruction = new List<Building>();
        charactersMarkedForDeletion = new List<Character>();

        ConstructionSites = new List<ConstructionSite>();

        tilesWithPlatformConstructionSites = new HashSet<Tile>();
        tilesWithSlabConstructionSites = new HashSet<Tile>();
        tilesWithSlabs = new HashSet<Tile>();

        crystalsDepositsReservations = new Dictionary<Character, float>();
        oreDepositsReservations = new Dictionary<Character, float>();

        mapChangedThisFrame = true;

        TimeLeft = StaticData.TimeLimit;

        unusedCharacterNames = StaticData.LoadNames();
        letters = StaticData.LoadLetters();
        digits = StaticData.LoadDigits();
    }

    public void UpdateModel(float deltaTime)
    {        
        if (initialTextShown == false)
        {
            Paused = true;
            CannotUnpause = true;

            initialTextShown = true;

            InitialAction();
        }

        if (mapChangedThisFrame)
        {
            Pathfinder.InvalidateGraph(modifiedTiles);
            mapChangedThisFrame = false;
            modifiedTiles.Clear();
        }

        Pathfinder.Process();

        if (Paused == false)
        {
            TimeLeft -= deltaTime;

            for (int i = 0; i < Characters.Count; i++)
            {
                Characters[i].UpdateCharacter(deltaTime);
            }

            for (int i = 0; i < Factories.Count; i++)
            {
                Factories[i].UpdateFactory(deltaTime);
            }

            for (int i = 0; i < ConstructionSites.Count; i++)
            {
                ConstructionSites[i].UpdateConstructionSite(deltaTime);
            }

            foreach (string need in Services.Keys)
            {
                for (int i = 0; i < Services[need].Count; i++)
                {
                    Services[need][i].UpdateService(deltaTime);
                }
            }

            foreach (Character c in crystalsDepositsReservations.Keys.ToList())
            {
                crystalsDepositsReservations[c] -= deltaTime;
                if (crystalsDepositsReservations[c] < 0f)
                {
                    crystalsDepositsReservations.Remove(c);
                }
            }

            foreach (Character c in oreDepositsReservations.Keys.ToList())
            {
                oreDepositsReservations[c] -= deltaTime;
                if (oreDepositsReservations[c] < 0f)
                {
                    oreDepositsReservations.Remove(c);
                }
            }            
        }

        for (int i = buildingsMarkedForDeconstruction.Count - 1;
             i >= 0;
             i--)
        {
            Building b = buildingsMarkedForDeconstruction[i];
            if (CheckIfBuildingIsReadyForDeconstruction(b))
            {
                StartBuildingDeconstruction(b);
                buildingsMarkedForDeconstruction.RemoveAt(i);
            }
        }

        for (int i = charactersMarkedForDeletion.Count - 1;
             i >= 0;
             i--)
        {
            Character c = charactersMarkedForDeletion[i];
            if (c.IsReadyForDeletion())
            {
                DeleteCharacter(c);
                charactersMarkedForDeletion.RemoveAt(i);
            }
        }

        if (gameEnded == false)
        {
            if (CheckVictoryConditions())
            {
                Paused = true;
                CannotUnpause = true;
                gameEnded = true;

                VictoryAction();
            }

            if (CheckDefeatConditions())
            {
                Paused = true;
                CannotUnpause = true;
                gameEnded = true;

                DefeatAction();
            }
        }       
    }

    bool CheckVictoryConditions()
    {
        return (AllResources[ResourceToGather] >= AmountToGather);
    }

    bool CheckDefeatConditions()
    {
        return (HumanNumber <= 0
                || TimeLeft <= 0f);
    }
  
    void VictoryAction()
    {
        GameManager.Instance.SoundManager.PlayVictorySound();

        Action action = () =>
        {
            Debug.Log("VICTORY - QUIT");            
            Application.Quit();
        };

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_victory_text",
            "s_victory_confirmation", action);
    }

    void DefeatAction()
    {
        GameManager.Instance.SoundManager.PlayDefeatSound();

        Action action = () =>
        {
            Debug.Log("DEFEAT - QUIT");
            Application.Quit();
        };

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_defeat_text",
            "s_defeat_confirmation", action);
    }

    void InitialAction()
    {
        Action action = () =>
        {
            Paused = false;
            CannotUnpause = false;
        };

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_start_text",
            "s_start_confirmation", action);
    }

    void CharacterDeathAction(string characterName)
    {
        Action action = () => {};

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_character_death" + characterName,
            "s_start_confirmation", action);
    }

#region CharactersManagement

    public bool CreateNewCharacter(TilePosition tilePosition, bool isRobot)
    {
        Tile t = GetTileFromPosition(tilePosition);

        if (Tile.CheckPassability(t))
        {
            Character c;
            if (isRobot)
            {
                c = new Character(GetRobotName(), t, robotBehaviourTree, isRobot);
                robotCounter++;
                RobotNumber++;
            }
            else
            {
                c = new Character(GetCharacterName(), t, humanBehaviourTree, isRobot);
                humanCounter++;
                HumanNumber++;
            }
            Characters.Add(c);

            GameManager.Instance.GenerateDisplayForCharacter(c, isRobot);
            Debug.Log("Dodano postać: " + c.CurrentTile.Position.ToString());
            return true;
        }
        return false;
    }

    string GetCharacterName()
    {
        int randomIndex = UnityEngine.Random.Range(0, unusedCharacterNames.Count);
        string result = unusedCharacterNames[randomIndex];
        unusedCharacterNames.RemoveAt(randomIndex);
        return result;
    }

    string GetRobotName()
    {
        string result =   GetRandomStringFromArray(letters)
                        + GetRandomStringFromArray(letters)
                        + "-"
                        + GetRandomStringFromArray(digits)
                        + GetRandomStringFromArray(digits);

        return result;
    }

    string GetRandomStringFromArray(string[] array)
    {
        int randomIndex = UnityEngine.Random.Range(0, array.Length);
        return array[randomIndex];
    }

    public void MarkCharacterForDeletion(Character c)
    {
        if (charactersMarkedForDeletion.Contains(c) == false)
        {
            c.StartPreparingForDeletion();
            charactersMarkedForDeletion.Add(c);            
        }
    }

    void DeleteCharacter(Character c)
    {
        Characters.Remove(c);
        if (c.IsRobot) RobotNumber--; else HumanNumber--;

        if (c.HasResource)
        {
            UnregisterResources(new Dictionary<int, int>(){ { c.Resource, 1 } });
            c.RemoveResource();
        }

        RemoveNaturalDepositReservations(c);

        if (c.Reservation != null)
        {
            if (c.Reservation.SourceStorage != null)
                c.Reservation.SourceStorage.RemoveResourceReservation(c);
            c.Reservation.TargetStorage.RemoveFreeSpaceReservation(c);

            c.ReservationUsed();
        }

        Pathfinder.RemoveCharacter(c);

        Debug.Log("Postać zmarła: " + c.Name);

        GameManager.Instance.RemoveDisplayForCharacter(c);
    }

    public bool IsCharacterOnTile(Tile tile)
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if ((Characters[i].CurrentTile == tile && Characters[i].MovementPercentage < 0.4f)
                || (Characters[i].NextTile == tile && Characters[i].MovementPercentage > 0.4f))
            {
                return true;
            }
        }
        return false;
    }

#endregion

#region ConstructionManagement

    public bool IsValidBuildingPosition(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        List<Tile> tilesToCheck = MapPrototypeToWorld(origin, rotation, prototype);

        for (int i = 0; i < tilesToCheck.Count; i++)
        {
            Tile tile = tilesToCheck[i];
            if (Tile.CheckPassability(tile)
                && !(prototype.CanBeBuiltOnSand == false && tile.Type == TileType.Sand)
                && !(prototype.CanBeBuiltOnRock == false && tile.Type == TileType.Rock)
                && !(prototype.CanBeBuiltOnPlatform == false && tile.Type == TileType.WalkableEmpty)
                && !(prototype.AllowToBuildOnTop == false && Platforms.ContainsKey(tile))
                && ((IsCharacterOnTile(tile) == false && tile.ReservedForAccess != true) || prototype.MovementCost != 0f)
                )
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        if (prototype.HasAccessTile
            && IsValidAccessTilePosition(
                MapPrototypeAccessTileToWorld(origin, rotation, prototype, false)) == false)
        {
            return false;
        }

        if (prototype.HasSecondAccessTile
            && IsValidAccessTilePosition(
                MapPrototypeAccessTileToWorld(origin, rotation, prototype, true)) == false)
        {
            return false;
        }

        if (prototype.Type == "Platform")
        {
            Tile platformTile = tilesToCheck[0];
            if (Platforms.ContainsKey(platformTile) 
                || tilesWithPlatformConstructionSites.Contains(platformTile))
            {
                return false;
            }
        }
        else if (prototype.Type == "Slab")
        {
            Tile slabTile = tilesToCheck[0];
            Tile platformTile = tilesToCheck[0];
            if (tilesWithSlabs.Contains(slabTile)
                || tilesWithSlabConstructionSites.Contains(slabTile))
            {
                return false;
            }
        }

        return true;
    }

    bool IsValidAccessTilePosition(Tile tile)
    {
        return (Tile.CheckPassability(tile));
    }

    public Tile MapPrototypeAccessTileToWorld(TilePosition origin, Rotation rotation,
                                              BuildingPrototype prototype, bool secondAccessTile)
    {
        if (secondAccessTile == false && prototype.HasAccessTile)
        {
            return GetTileFromPosition(MapNormalizedPositionToWorld(
                       prototype.NormalizedAccessTilePosition,
                       origin, rotation));
        }
        else if (secondAccessTile && prototype.HasSecondAccessTile)
        {
            return GetTileFromPosition(MapNormalizedPositionToWorld(
                       prototype.NormalizedSecondAccessTilePosition,
                       origin, rotation));
        }
        return null;
    }

    public List<Tile> MapPrototypeToWorld(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        List<Tile> tiles = new List<Tile>(prototype.NormalizedTilePositions.Count);

        for (int i = 0; i < prototype.NormalizedTilePositions.Count; i++)
        {
            TilePosition tp = prototype.NormalizedTilePositions[i];
            Tile tile = GetTileFromPosition(MapNormalizedPositionToWorld(tp, origin, rotation));
            tiles.Add(tile);
        }
        return tiles;
    }

    public TilePosition MapNormalizedPositionToWorld(TilePosition normalizedPosition, TilePosition origin, Rotation rotation)
    {
        TilePosition result;
        if (rotation == Rotation.N)
        {
            result = new TilePosition(
            origin.X + normalizedPosition.X,
            origin.Y + normalizedPosition.Y,
            origin.Height + normalizedPosition.Height);
        }
        else if (rotation == Rotation.E)
        {
            result = new TilePosition(
            origin.X + normalizedPosition.Y,
            origin.Y - normalizedPosition.X,
            origin.Height + normalizedPosition.Height);
        }
        else if (rotation == Rotation.S)
        {
            result = new TilePosition(
            origin.X - normalizedPosition.X,
            origin.Y - normalizedPosition.Y,
            origin.Height + normalizedPosition.Height);
        }
        else // W
        {
            result = new TilePosition(
            origin.X - normalizedPosition.Y,
            origin.Y + normalizedPosition.X,
            origin.Height + normalizedPosition.Height);
        }
        return result;
    }

    public BuildingPrototype GetBuildingPrototype(string type)
    {
        BuildingPrototype prototype = null;
        for (int i = 0; i < buildingPrototypes.Count; i++)
        {
            if (buildingPrototypes[i].Type == type) prototype = buildingPrototypes[i];
        }
        if (prototype == null)
        {
            Debug.LogWarning("Nie ma takiego rodzaju budynku: " + type);
        }
        return prototype;
    }

    public ConstructionSite PlaceNewConstructionSite(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        if (IsValidBuildingPosition(origin, rotation, prototype) == false)
        {
            Debug.Log("Nie można postawić budynku: lokacja jest nieodpowiednia.");
            return null;
        }
        else
        {
            List<Tile> tilesForBuilding = MapPrototypeToWorld(origin, rotation, prototype);

            Tile accessTile = MapPrototypeAccessTileToWorld(origin, rotation, prototype, false);
            if (accessTile != null) { accessTile.ReservedForAccess = true; }
            Tile secondAccessTile = MapPrototypeAccessTileToWorld(origin, rotation, prototype, true);
            if (secondAccessTile != null) { secondAccessTile.ReservedForAccess = true; }

            Building newBuilding = new Building(prototype, rotation, tilesForBuilding, accessTile, secondAccessTile);
            Buildings.Add(newBuilding);

            if (prototype.ConstructionWithoutScaffolding == false)
            {
                ApplyNewMovementCostsToTiles(newBuilding);
            }

            if (prototype.Type == "Platform")
            {
                tilesWithPlatformConstructionSites.Add(newBuilding.Tiles[0]);
            }
            else if (prototype.Type == "Slab")
            {
                tilesWithSlabs.Add(newBuilding.Tiles[0]);
            }

            ConstructionSite newConstructionSite = new ConstructionSite(newBuilding, prototype, false);
            ConstructionSites.Add(newConstructionSite);

            GameManager.Instance.SoundManager.PlayStartConstructionSound();

            GameManager.Instance.ShowConstructionSite(newConstructionSite);

            // Debug.Log("Nowy plac budowy: " + newBuilding.Tiles[0].Position.ToString());

            return newConstructionSite;
        }
    }

    void ApplyNewMovementCostsToTiles(Building building)
    {
        for (int i = 0; i < building.Tiles.Count; i++)
        {
            building.Tiles[i].MovementCost = building.Prototype.MovementCost;
            modifiedTiles.Add(building.Tiles[i]);

            if (building.Prototype.DisallowDiagonalMovement)
            {
                building.Tiles[i].AllowDiagonal = false;
            }
        }

        if (building.Prototype.WalkableOnTop)
        {
            for (int i = 0; i < building.Tiles.Count; i++)
            {
                Tile tileOnTop = building.Tiles[i].GetUpperNeighbour();
                if (tileOnTop != null)
                {
                    tileOnTop.Type = TileType.WalkableEmpty;
                    tileOnTop.MovementCost = building.Prototype.MovementCostOnTop;
                    modifiedTiles.Add(tileOnTop);
                }
            }
        }

        mapChangedThisFrame = true;
    }

    void RemoveMovementCostsModificationByBuilding(Building building)
    {
        for (int i = 0; i < building.Tiles.Count; i++)
        {
            building.Tiles[i].MovementCost = TileTypesMovementCosts[building.Tiles[i].Type];
            modifiedTiles.Add(building.Tiles[i]);
            building.Tiles[i].AllowDiagonal = true;
        }

        if (building.Prototype.WalkableOnTop)
        {
            for (int i = 0; i < building.Tiles.Count; i++)
            {
                Tile tileOnTop = building.Tiles[i].GetUpperNeighbour();
                if (tileOnTop != null)
                {
                    tileOnTop.Type = TileType.Empty;
                    tileOnTop.MovementCost = TileTypesMovementCosts[TileType.Empty];
                    modifiedTiles.Add(tileOnTop);
                }
            }
        }

        mapChangedThisFrame = true;
    }

    public Building InstantBuild(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        ConstructionSite site = PlaceNewConstructionSite(origin, rotation, prototype);
        if (site != null)
        {
            // Debug.Log("InstantBuild");
            return FinishConstruction(site);
        }
        else
        {
            return null;
        }
    }

    public void MarkBuildingForDenconstruction(Building building)
    {
        if (building == null) return;

        if (building.Prototype.CanBeDeconstructed == false) return;

        if (buildingsMarkedForDeconstruction.Contains(building) == false)
        {
            buildingsMarkedForDeconstruction.Add(building);
            if (building.Module != null) building.Module.StartDeconstructionPreparation();
        }
        else
        {            
            Debug.Log("Próbowano dwa razy zarządzić dekonstrukcję tego samego budynku");
        }
    }

    bool CheckIfBuildingIsReadyForDeconstruction(Building building)
    {
        if (building.Module != null)
        {
            if (building.Module is ConstructionSite)
            {
                Debug.LogWarning("Próbujemy zacząć dekonstruować budynek, który już ma przypisany plac");
            }

            return (building.Module.IsReadyForDeconstruction());
        }
        else
        {
            return true;
        }
    }

    void StartBuildingDeconstruction(Building building)
    {
        ConstructionSite deconstructionSite = null;

        bool existingSite = false;

        // Na wypadek, gdybyśmy zlecali rozbiórkę na nieukończonej budowie
        foreach (ConstructionSite site in ConstructionSites)
        {
            if (site.Building == building)
            {
                existingSite = true;
                site.CancelConstruction();
                deconstructionSite = site;                                       
            }            
        }

        if (deconstructionSite == null)
        {
            deconstructionSite = new ConstructionSite(building, building.Prototype, true);
            ConstructionSites.Add(deconstructionSite);
        }
    
        if (building.Prototype.ConstructionWithoutScaffolding)
        {
            RemoveMovementCostsModificationByBuilding(building);
        }

        Tile tileToUpdatePlatformDisplay = null;
        if (Platforms.Keys.Contains(building.Tiles[0]))
        {
            tileToUpdatePlatformDisplay = building.Tiles[0];
        }

        UnregisterBuildingInDeconstruction(building);

        if (tileToUpdatePlatformDisplay != null)
        {
            GameManager.Instance.UpdatePlatformDisplay(tileToUpdatePlatformDisplay);
        }

        Buildings.Remove(building);
        building.RemoveModule();
        building.IsDeconstructed = true;

        mapChangedThisFrame = true;

        Debug.Log("Usunięto budynek: " + building.Tiles[0].Position.ToString());

        if (existingSite == false)
        {
            GameManager.Instance.SoundManager.PlayStartDeconstructionSound();
        }

        GameManager.Instance.RemoveDisplayForBuilding(building);
        GameManager.Instance.ShowConstructionSite(deconstructionSite);

        if (existingSite && deconstructionSite.GetCompletionPercentage() < 0.01f)
        {
            foreach (Character c in deconstructionSite.InputStorage.PendingResources.Keys)
            {
                c.ReservationUsed();
            }

            FinishDeconstruction(deconstructionSite);
        }
    }

    public void FinishDeconstruction(ConstructionSite site)
    {
        if (ConstructionSites.Contains(site) == false || site.Building == null)
        {
            Debug.Log("Próba ukończenia dekonstrukcji przez niezarejestrowany plac budowy");
            return;
        }

        Building buildingToDeconstruct = site.Building;

        CancelBuildingReservationForAccess(buildingToDeconstruct);
       
        if (buildingToDeconstruct.Prototype.ConstructionWithoutScaffolding == false)
        {
            RemoveMovementCostsModificationByBuilding(buildingToDeconstruct);
        }
    
        CancelBuildingReservationForAccess(buildingToDeconstruct);
        
        GameManager.Instance.RemoveConstructionSiteDisplay(site);

        site.Building = null;
        ConstructionSites.Remove(site);
    }

    public void CancelBuildingDeconstruction(Building building)
    {
        if (buildingsMarkedForDeconstruction.Contains(building))
        {
            buildingsMarkedForDeconstruction.Remove(building);
            if (building.Module != null) building.Module.CancelDeconstructionPreparation();
        }
        else
        {
            Debug.Log("Próbowano odwołać niezleconą rozbiórkę budynku");
        }
    }
    
    public Building FinishConstruction(ConstructionSite site)
    {
        if (ConstructionSites.Contains(site) == false)
        {
            Debug.Log("Próba ukończenia konstrukcji przez niezarejestrowany plac budowy");
            return null;
        }

        Building buildingToConstruct = site.Building;

        buildingToConstruct.LoadDataForFinishedBuilding();

        if (buildingToConstruct.Prototype.ConstructionWithoutScaffolding)
        {
            ApplyNewMovementCostsToTiles(buildingToConstruct);
        }

        RegisterConstructedBuilding(buildingToConstruct);

        GameManager.Instance.RemoveConstructionSiteDisplay(site);

        TilePosition positionForBuildingDisplay = site.Building.Tiles[0].Position -
                MapNormalizedPositionToWorld(site.Prototype.NormalizedTilePositions[0],
                                             new TilePosition(0, 0, 0),
                                             buildingToConstruct.Rotation);

        site.Building = null;
        ConstructionSites.Remove(site);

        GameManager.Instance.SoundManager.PlayFinishConstructionSound();

        GameManager.Instance.ShowBuilding(buildingToConstruct, positionForBuildingDisplay);        

        return buildingToConstruct;
    }

    void CancelBuildingReservationForAccess(Building building)
    {
        CancelBuildingReservationForAccess(building, true);
        CancelBuildingReservationForAccess(building, false);
    }

    void CancelBuildingReservationForAccess(Building building, bool secondAccessTile)
    {
        Tile tileToCancelReservation = building.GetAccessTile(secondAccessTile);
        if (tileToCancelReservation != null)
        {
            for (int i = 0; i < Buildings.Count; i++)
            {
                Tile accessTileToCheck = Buildings[i].GetAccessTile();
                if (accessTileToCheck != null
                   && accessTileToCheck == tileToCancelReservation
                   && Buildings[i] != building)
                {
                    return;
                }
            }
            tileToCancelReservation.ReservedForAccess = false;
        }
    }

    void RegisterConstructedBuilding(Building building)
    {        
        if (building.Prototype.RestrictedResources.Contains(0) == false)
        {
            totalSpaceForCrystals += building.Prototype.MaxStorage;
        }
        else if (building.Prototype.ConsumedResources != null
                 && building.Prototype.ConsumedResources.ContainsKey(3))
        {            
            totalSpaceForOre += building.Prototype.ConsumedResources[3];
        }
        else if (building.Type == "Stairs")
        {
            Stairs.Add(building);
        }
        else if (building.Type == "Platform")
        {
            tilesWithPlatformConstructionSites.Remove(building.Tiles[0]);
            Platforms.Add(building.Tiles[0], building);
        }
        else if (building.Type == "Slab")
        {
            tilesWithSlabConstructionSites.Remove(building.Tiles[0]);
            tilesWithSlabs.Add(building.Tiles[0]);
        }

        IBuildingModule module = building.Module;
        if (module != null)
        {
            if (module is Factory)
            {
                if (((Factory)module).Prototype.IsNaturalDeposit)
                {
                    NaturalDeposits.Add((Factory)module);
                }
                else
                {
                    NonDepositFactories.Add((Factory)module);
                }

                Factories.Add((Factory)module);
            }
            if (module is Storage)
            {
                Storages.Add((Storage)module);
            }
            if (module is Service)
            {
                string need = ((Service)module).NeedFulfilled;
                if (Services.ContainsKey(need) == false)
                {
                    Services.Add(need, new List<Service>());
                }
                Services[need].Add((Service)module);
            }
        }
    }

    void UnregisterBuildingInDeconstruction(Building building)
    {
        if (building.Prototype.RestrictedResources.Contains(0) == false)
        {
            totalSpaceForCrystals -= building.Prototype.MaxStorage;
        }
        else if (building.Prototype.ConsumedResources != null
                 && building.Prototype.ConsumedResources.ContainsKey(3))
        {
            totalSpaceForOre -= building.Prototype.ConsumedResources[3];
        }
        else if (building.Type == "Stairs")
        {
            Stairs.Remove(building);
        }
        else if (building.Type == "Platform")
        {
            if (Platforms.ContainsKey(building.Tiles[0]))
                Platforms.Remove(building.Tiles[0]);
        }
        else if (building.Type == "Slab")
        {
            tilesWithSlabs.Remove(building.Tiles[0]);
        }        

        IBuildingModule module = building.Module;
        if (module != null)
        {
            if (module is Factory)
            {
                if (((Factory)module).Prototype.IsNaturalDeposit)
                {
                    NaturalDeposits.Remove((Factory)module);
                }
                else
                {
                    NonDepositFactories.Remove((Factory)module);
                }

                Factories.Remove((Factory)module);
            }
            if (module is Storage)
            {
                Storages.Remove((Storage)module);
            }
            if (module is Service)
            {
                string need = ((Service)module).NeedFulfilled;
                Services[need].Remove((Service)module);
                if (Services[need].Count == 0)
                {
                    Services.Remove(need);
                }
            }
        }
    }

#endregion

#region ResourcesManagement

    public ResourceReservation GetReservationForFillingInput(Character character)
    {
        ResourceReservation result = null;

        foreach (string need in Services.Keys)
        {
            List<Service> services = Services[need];

            if (services.Count > 0)
            {
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    int index = UnityEngine.Random.Range(0, services.Count);
                    Service service = services[index];

                    if (character.AreBothAccessTilesMarkedAsInaccessbile(service))
                        continue;

                    result = GetReservationForFillingInput(character, service.InputStorage);
                    if (result != null) return result;
                }
            }
        }

        if (ConstructionSites.Count > 0)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                int index = UnityEngine.Random.Range(0, ConstructionSites.Count);
                ConstructionSite site = ConstructionSites[index];

                if (site.Halted || character.AreBothAccessTilesMarkedAsInaccessbile(site))
                    continue;

                result = GetReservationForFillingInput(character, site.InputStorage);
                if (result != null) return result;
            }
        }

        if (Factories.Count > 0)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                int index = UnityEngine.Random.Range(0, Factories.Count);
                Factory factory = Factories[index];

                if (factory.Halted || character.AreBothAccessTilesMarkedAsInaccessbile(factory))
                    continue;

                result = GetReservationForFillingInput(character, factory.InputStorage);
                if (result != null) return result;
            }
        }

        return result;
    }

    ResourceReservation GetReservationForFillingInput(Character character, StorageWithRequirements storageToFill)
    {
        if (character.Reservation != null
            || storageToFill.AreRequirementsMet
            || storageToFill.RequiresEmptying)
        {
            return null;
        }

        ResourceReservation newReservation = null;

        bool storageToFillSecondAccessTile = (character.IsTileMarkedAsInaccessible(storageToFill.GetAccessTile(false)));

        foreach (int resourceID in storageToFill.MissingResources.Keys)
        {
            // Czy nie ma potrzebnych zasobów w jakiejś fabryce do opróżnienia?
            foreach (Factory factoryToCheck in Factories)
            {
                if (character.AreBothAccessTilesMarkedAsInaccessbile(factoryToCheck))
                    continue;

                bool factoryToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                        factoryToCheck.GetAccessTile(false)));

                if (factoryToCheck.OutputStorage.Resources.ContainsKey(resourceID))
                {
                    if (factoryToCheck.OutputStorage.CanReserveResource(resourceID, character)
                        && storageToFill.CanReserveFreeSpace(resourceID, character))
                    {                        
                        newReservation = new ResourceReservation(
                            factoryToCheck.OutputStorage,
                            factoryToCheckSecondAccessTile,
                            storageToFill,
                            storageToFillSecondAccessTile,
                            resourceID);
                        break;
                    }
                }

                if (factoryToCheck.InputStorage.RequiresEmptying)
                {
                    if (factoryToCheck.InputStorage.CanReserveResource(resourceID, character)
                        && storageToFill.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            factoryToCheck.InputStorage,
                            factoryToCheckSecondAccessTile,
                            storageToFill,
                            storageToFillSecondAccessTile,
                            resourceID);
                        break;
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }

            // Czy nie ma potrzebnych zasobów w jakimś magazynie?
            foreach (Storage storageToCheck in Storages)
            {
                if (character.AreBothAccessTilesMarkedAsInaccessbile(storageToCheck))
                    continue;

                bool storageToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                       storageToCheck.GetAccessTile(false)));

                if (storageToCheck.Resources.ContainsKey(resourceID))
                {
                    if (storageToCheck.CanReserveResource(resourceID, character)
                        && storageToFill.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToCheck,
                            storageToCheckSecondAccessTile,
                            storageToFill, 
                            storageToFillSecondAccessTile,
                            resourceID);
                        break;
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }            
        }

        if (newReservation != null)
        {
            if (newReservation.SourceStorage.ReserveResource(newReservation.Resource, character)
               && newReservation.TargetStorage.ReserveFreeSpace(newReservation.Resource, character))
            {
                return newReservation;
            }
            else
            {
                newReservation.SourceStorage.RemoveResourceReservation(character);
                newReservation.TargetStorage.RemoveFreeSpaceReservation(character);
                return null;
            }
        }

        // Nie udało się nic znaleźć
        return null;        
    }
    
    public ResourceReservation GetReservationForEmptying(Character character)
    {
        ResourceReservation result = null;

        foreach (Factory factory in Factories)
        {
            if (character.AreBothAccessTilesMarkedAsInaccessbile(factory))
                continue;

            result = GetReservationForEmptying(character, factory.OutputStorage);

            if (result == null && factory.InputStorage.RequiresEmptying)
            {
                result = GetReservationForEmptying(character, factory.InputStorage);
            }

            if (result != null) return result;
        }

        foreach (ConstructionSite site in ConstructionSites)
        {
            if (character.AreBothAccessTilesMarkedAsInaccessbile(site))
                continue;

            if (site.DeconstructionMode)
            {
                result = GetReservationForEmptying(character, site.OutputStorage);
                if (result != null) return result;
            }
        }
        
        foreach (Storage storage in Storages)
        {
            if (character.AreBothAccessTilesMarkedAsInaccessbile(storage))
                continue;

            if (storage.RequiresEmptying)
            {
                result = GetReservationForEmptying(character, storage);
                if (result != null) return result;
            }
        }

        foreach (string need in Services.Keys)
        {
            List<Service> services = Services[need];

            foreach (Service serivce in services)
            {
                if (character.AreBothAccessTilesMarkedAsInaccessbile(serivce))
                    continue;

                if (serivce.InputStorage.RequiresEmptying)
                {
                    result = GetReservationForEmptying(character, serivce.InputStorage);
                    if (result != null) return result;
                }
            }
        }

        return result;
    }

    ResourceReservation GetReservationForEmptying(Character character, Storage storageToEmpty)
    {
        if (character.Reservation != null || storageToEmpty.IsEmpty)
        {
            return null;
        }

        ResourceReservation newReservation = null;

        bool storageToEmptySecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                storageToEmpty.GetAccessTile(false)));

        foreach (int resourceID in storageToEmpty.Resources.Keys)
        {
            // Czy jakaś usługa nie potrzebuje tego zasobu?
            foreach (string need in Services.Keys)
            {
                List<Service> services = Services[need];

                foreach(Service serviceToCheck in services)
                {
                    if (character.AreBothAccessTilesMarkedAsInaccessbile(serviceToCheck)
                        || serviceToCheck.InputStorage.RequiresEmptying) continue;

                    bool serviceToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                            serviceToCheck.GetAccessTile(false)));

                    if (serviceToCheck.InputStorage.AreRequirementsMet == false
                        && serviceToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                    {
                        if (storageToEmpty.CanReserveResource(resourceID, character)
                            && serviceToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                        {
                            newReservation = new ResourceReservation(
                                storageToEmpty,
                                storageToEmptySecondAccessTile,
                                serviceToCheck.InputStorage,
                                serviceToCheckSecondAccessTile,
                                resourceID);
                            break;
                        }
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }

            // Czy jakaś fabryka nie potrzebuje tego zasobu?
            foreach (Factory factoryToCheck in Factories)
            {
                if (factoryToCheck.Halted 
                    || character.AreBothAccessTilesMarkedAsInaccessbile(factoryToCheck)
                    || factoryToCheck.InputStorage.RequiresEmptying) continue;

                bool factoryToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                        factoryToCheck.GetAccessTile(false)));

                if (factoryToCheck.InputStorage.AreRequirementsMet == false
                    && factoryToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && factoryToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty,
                            storageToEmptySecondAccessTile,
                            factoryToCheck.InputStorage,
                            factoryToCheckSecondAccessTile,
                            resourceID);
                        break;
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }

            // Czy jakiś plac budowy nie potrzebuje tego zasobu?
            foreach (ConstructionSite siteToCheck in ConstructionSites)
            {
                if (siteToCheck.DeconstructionMode
                    || character.AreBothAccessTilesMarkedAsInaccessbile(siteToCheck))
                    continue;

                bool siteToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                    siteToCheck.GetAccessTile(false)));

                if (siteToCheck.InputStorage.AreRequirementsMet == false
                    && siteToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && siteToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty,
                            storageToEmptySecondAccessTile,
                            siteToCheck.InputStorage,
                            siteToCheckSecondAccessTile,
                            resourceID);
                        break;
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }

            // Czy w jakimś magazynie jest wolne miejsce?
            List<Storage> sortedStorageList = new List<Storage>(Storages);
            sortedStorageList.OrderBy(s => character.CurrentTile.DistanceTo(s.GetAccessTile()));

            foreach (Storage storageToCheck in sortedStorageList)
            {
                if (storageToCheck.RequiresEmptying
                    || character.AreBothAccessTilesMarkedAsInaccessbile(storageToCheck))
                    continue;

                bool storageToCheckSecondAccessTile = (character.IsTileMarkedAsInaccessible(
                                                        storageToCheck.GetAccessTile(false)));

                if (storageToCheck.UnreservedFreeSpace > 0)
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && storageToCheck.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty,
                            storageToEmptySecondAccessTile,
                            storageToCheck,
                            storageToCheckSecondAccessTile,
                            resourceID);
                        break;
                    }
                }
            }

            if (newReservation != null)
            {
                break;
            }
        }

        if (newReservation != null)
        {
            if (newReservation.SourceStorage.ReserveResource(newReservation.Resource, character)
                && newReservation.TargetStorage.ReserveFreeSpace(newReservation.Resource, character))
            {
                return newReservation;
            }
            else
            {
                newReservation.SourceStorage.RemoveResourceReservation(character);
                newReservation.TargetStorage.RemoveFreeSpaceReservation(character);
                return null;
            }
        }

        // Nie udało się nic znaleźć
        return null;
    }

    public IWorkplace GetAvailableWorkplace(Character character)
    {
        if (ConstructionSites.Count > 0)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                int index = UnityEngine.Random.Range(0, ConstructionSites.Count);

                if (character.AreBothAccessTilesMarkedAsInaccessbile(ConstructionSites[index]))
                {
                    // Debug.Log("inaccessible: " + ConstructionSites[index].GetAccessTile().ToString());
                    continue;
                }

                if (ConstructionSites[index].CanReserveJob(character))
                {
                    return ConstructionSites[index];
                }
            }
        }

        if (NonDepositFactories.Count > 0)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int index = UnityEngine.Random.Range(0, NonDepositFactories.Count);

                Factory f = NonDepositFactories[index];

                if (character.AreBothAccessTilesMarkedAsInaccessbile(f))
                {
                    continue;
                }

                if (f.CanReserveJob(character))
                {
                    return f;
                }
            }
        }

        if (NaturalDeposits.Count > 0)
        {
            for (int attempt = 0; attempt < 30; attempt++)
            {
                int index = UnityEngine.Random.Range(0, NaturalDeposits.Count);

                Factory f = NaturalDeposits[index];

                if (character.AreBothAccessTilesMarkedAsInaccessbile(f))
                {
                    continue;
                }

                bool crystals = f.Prototype.ProducedResources.ContainsKey(0);

                if (CanReserveNaturalDeposit(character, crystals) && f.CanReserveJob(character))
                {
                    return f;
                }          
            }
        }

        return null;
    }

    public ResourceReservation GetReservationToStoreCurrentResource(Character character)
    {
        ResourceReservation result = null;
        foreach (Storage s in Storages)
        {
            if (s.CanReserveFreeSpace(character.Resource, character)
                && character.IsTileMarkedAsInaccessible(s.GetAccessTile()) == false)
            {
                result = new ResourceReservation(null, false, s, false, character.Resource);

                return result;
            }
        }
        return result;
    }

    public Service GetAnyAvailableService(string need, Character character)
    {
        if (Services.ContainsKey(need) == false || Services[need].Count == 0) return null;
        foreach (Service s in Services[need])
        {
            if (s.CanReserveService(character)
                && character.IsTileMarkedAsInaccessible(s.GetAccessTile()) == false)
            {
                return s;
            }
        }
        return null;
    }

    public Service GetClosestAvailableService(string need, Character character)
    {
        if (Services.ContainsKey(need) == false || Services[need].Count == 0) return null;
        
        List<Service> availableServices = new List<Service>();

        foreach (Service s in Services[need])
        {
            if (s.CanReserveService(character)
                && character.IsTileMarkedAsInaccessible(s.GetAccessTile()) == false)
            {
                availableServices.Add(s);
            }
        }

        Service result = null;
        float distance = Mathf.Infinity;
        foreach (Service s in availableServices)
        {
            float newDistance = character.CurrentTile.DistanceTo(s.GetAccessTile());
            if (newDistance < distance)
            {
                distance = newDistance;
                result = s;
            }
        }

        return result;
    }
   
    public void RegisterResources(Dictionary<int, int> newResources)
    {
        if (newResources == null) return;

        foreach(int resourceID in newResources.Keys)
        {
            AllResources[resourceID] = AllResources[resourceID] + newResources[resourceID];
        }
    }

    public void UnregisterResources(Dictionary<int, int> resourcesToRemove)
    {
        if (resourcesToRemove == null) return;

        foreach (int resourceID in resourcesToRemove.Keys)
        {
            if (AllResources[resourceID] >= resourcesToRemove[resourceID])
            {
                AllResources[resourceID] = AllResources[resourceID] - resourcesToRemove[resourceID];
            }
            else
            {
                Debug.Log("Próbujemy usuwać zasoby, których nie dodaliśmy");
            }
        }
    }

    public void PlaceNaturalResources()
    {
        if (oreDeposits != null)
        {
            BuildingPrototype orePrototype = GetBuildingPrototype("OreDeposit");
            foreach (TilePosition position in oreDeposits.Keys)
            {
                Building deposit;
                deposit = InstantBuild(position, RotationMethods.GetRandomRotation(), orePrototype);
                if (deposit != null)
                {
                    ((Factory)deposit.Module).SetRemainingProductionCycles(oreDeposits[position]);
                }
            }
        }
        if (crystalsDeposits != null)
        {
            BuildingPrototype crystalsPrototype = GetBuildingPrototype("CrystalsDeposit");
            foreach (TilePosition position in crystalsDeposits.Keys)
            {
                Building deposit;
                deposit = InstantBuild(position, RotationMethods.GetRandomRotation(), crystalsPrototype);
                if (deposit != null)
                {
                    ((Factory)deposit.Module).SetRemainingProductionCycles(crystalsDeposits[position]);
                }
            }
        }        
    }

    bool CanReserveNaturalDeposit(Character c, bool crystals)
    {
        if (crystals)
        {
            if (crystalsDepositsReservations.ContainsKey(c)) return true;

            return (totalSpaceForCrystals - AllResources[0] > crystalsDepositsReservations.Count);
        }
        else
        {
            if (oreDepositsReservations.ContainsKey(c)) return true;

            return (totalSpaceForOre - AllResources[3] > oreDepositsReservations.Count);
        }
    }

    public bool ReserveNaturalDeposit(Character c, IWorkplace deposit)
    {
        bool crystals = deposit.Building.Prototype.ProducedResources.ContainsKey(0);
        return ReserveNaturalDeposit(c, crystals);
    }

    public bool ReserveNaturalDeposit(Character c, bool crystals)
    {
        if (CanReserveNaturalDeposit(c, crystals) == false) return false;

        if (crystals)
        {
            if (crystalsDepositsReservations.ContainsKey(c))
            {
                crystalsDepositsReservations[c] = naturalDepositsReservationTimer;
            }
            else
            {
                crystalsDepositsReservations.Add(c, naturalDepositsReservationTimer);
            }           
        }
        else
        {
            if (oreDepositsReservations.ContainsKey(c))
            {
                oreDepositsReservations[c] = naturalDepositsReservationTimer;
            }
            else
            {
                oreDepositsReservations.Add(c, naturalDepositsReservationTimer);
            }
        }
        return true;        
    }

    public void RemoveNaturalDepositReservations(Character c)
    {
        if (oreDepositsReservations.Keys.Contains(c)) oreDepositsReservations.Remove(c);
        if (crystalsDepositsReservations.Keys.Contains(c)) crystalsDepositsReservations.Remove(c);
    }

#endregion

#region TileUtilities

    public Tile GetTileFromPosition(int x, int y, int height)
    {
        return GetTileFromPosition(new TilePosition(x, y, height));
    }

    public Tile GetTileFromPosition(TilePosition tilePosition)
    {
        if (CheckTilePosition(tilePosition) == false)
        {
            return null;
        }
        
        if (Tiles[tilePosition.X, tilePosition.Y, tilePosition.Height] == null)
        {
            return null;
        }

        return Tiles[tilePosition.X, tilePosition.Y, tilePosition.Height];
    }

    public bool CheckTilePosition(TilePosition tilePosition)
    {
        if (tilePosition.X < 0 || tilePosition.Y < 0 || tilePosition.Height < 0 ||
            tilePosition.X >= XSize || tilePosition.Y >= YSize || tilePosition.Height >= Height)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

#endregion

}