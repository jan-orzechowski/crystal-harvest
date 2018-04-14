using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
    public List<ConstructionSite> ConstructionSites { get; protected set; }
    public Dictionary<string, List<Service>> Services { get; protected set; }
    public List<Building> Stairs { get; protected set; }
    public Dictionary<Tile, Building> Platforms { get; protected set; }

    List<Building> buildingsMarkedForDeconstruction;

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
        Services = new Dictionary<string, List<Service>>();
        Stairs = new List<Building>();
        Platforms = new Dictionary<Tile, Building>();

        buildingsMarkedForDeconstruction = new List<Building>();

        ConstructionSites = new List<ConstructionSite>();

        mapChangedThisFrame = true;
    }

    public void UpdateModel(float deltaTime)
    {
        if (mapChangedThisFrame)
        {
            Pathfinder.InvalidateGraph(modifiedTiles);
            mapChangedThisFrame = false;
            modifiedTiles.Clear();
        }

        Pathfinder.Process();

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
                c = new Character(("Robot #" + robotCounter), t, robotBehaviourTree, isRobot);
                robotCounter++;
                RobotNumber++;
            }
            else
            {
                c = new Character(("Human #" + humanCounter), t, humanBehaviourTree, isRobot);
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

            ConstructionSite newConstructionSite = new ConstructionSite(newBuilding, prototype, false);
            ConstructionSites.Add(newConstructionSite);

            GameManager.Instance.ShowConstructionSite(newConstructionSite);

            Debug.Log("Nowy plac budowy: " + newBuilding.Tiles[0].Position.ToString());

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

    public void MarkBuildingToDenconstruction(Building building)
    {
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
        ConstructionSite deconstructionSite = new ConstructionSite(building, building.Prototype, true);
        ConstructionSites.Add(deconstructionSite);
    
        if (building.Prototype.ConstructionWithoutScaffolding)
        {
            RemoveMovementCostsModificationByBuilding(building);
        }

        UnregisterBuildingInDeconstruction(building);

        Buildings.Remove(building);
        building.RemoveModule();

        mapChangedThisFrame = true;

        Debug.Log("Usunięto budynek: " + building.Tiles[0].Position.ToString());
        
        GameManager.Instance.RemoveDisplayForBuilding(building);
        GameManager.Instance.ShowConstructionSite(deconstructionSite);
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
        if (building.Type == "Stairs")
        {
            Stairs.Add(building);
        }
        else if (building.Type == "Platform")
        {
            Platforms.Add(building.Tiles[0], building);
        }

        IBuildingModule module = building.Module;
        if (module != null)
        {
            if (module is Factory)
            {
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
        if (building.Type == "Stairs")
        {
            Stairs.Remove(building);
        }
        else if (building.Type == "Platform")
        {
            if (Platforms.ContainsKey(building.Tiles[0]))
                Platforms.Remove(building.Tiles[0]);
        }

        IBuildingModule module = building.Module;
        if (module != null)
        {
            if (module is Factory)
            {
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

    public bool GetReservationForFillingInput(Character character)
    {
        foreach (Factory factory in Factories)
        {
            if (factory.Halted) continue;

            if (GetReservationForFillingInput(character, factory.InputStorage)) return true;
        }

        foreach (ConstructionSite site in ConstructionSites)
        {
            if (site.Halted) continue;

            if (GetReservationForFillingInput(character, site.InputStorage)) return true;
        }

        return false;
    }

    public bool GetReservationForFillingInput(Character character, StorageWithRequirements storageToFill)
    {
        if (character.Reservation != null
            || storageToFill.AreRequirementsMet
            || storageToFill.RequiresEmptying)
        {
            return false;
        }

        ResourceReservation newReservation = null;
        
        foreach (int resourceID in storageToFill.MissingResources.Keys)
        {
            // Czy nie ma potrzebnych zasobów w jakiejś fabryce do opróżnienia?
            foreach (Factory factoryToCheck in Factories)
            {                
                if (factoryToCheck.OutputStorage.Resources.ContainsKey(resourceID))
                {
                    if (factoryToCheck.OutputStorage.CanReserveResource(resourceID, character)
                        && storageToFill.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            factoryToCheck.OutputStorage,
                            storageToFill, 
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
                            storageToFill,
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
                if (storageToCheck.Resources.ContainsKey(resourceID))
                {
                    if (storageToCheck.CanReserveResource(resourceID, character)
                        && storageToFill.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToCheck,
                            storageToFill, 
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
                character.SetNewReservation(newReservation);
                return true;
            }
            else
            {
                newReservation.SourceStorage.RemoveResourceReservation(character);
                newReservation.TargetStorage.RemoveFreeSpaceReservation(character);
                return false;
            }
        }

        // Nie udało się nic znaleźć
        return false;        
    }
    
    public bool GetReservationForEmptying(Character character)
    {
        foreach (Factory factory in Factories)
        {
            if (GetReservationForEmptying(character, factory.OutputStorage)
                || (factory.InputStorage.RequiresEmptying 
                    && GetReservationForEmptying(character, factory.InputStorage)))
            {
                return true;
            }
        }

        foreach (ConstructionSite site in ConstructionSites)
        {
            if (site.DeconstructionMode && GetReservationForEmptying(character, site.OutputStorage))
            {
                return true;
            }
        }

        foreach (Storage storage in Storages)
        {
            if (storage.RequiresEmptying
                && GetReservationForEmptying(character, storage))
            {
                return true;
            }
        }

        return false;
    }

    public bool GetReservationForEmptying(Character character, Storage storageToEmpty)
    {
        if (character.Reservation != null || storageToEmpty.IsEmpty)
        {
            return false;
        }

        ResourceReservation newReservation = null;

        foreach (int resourceID in storageToEmpty.Resources.Keys)
        {
            // Czy jakaś fabryka nie potrzebuje tego zasobu?
            foreach (Factory factoryToCheck in Factories)
            {
                if (factoryToCheck.Halted || factoryToCheck.InputStorage.RequiresEmptying) continue;

                if (factoryToCheck.InputStorage.AreRequirementsMet == false
                    && factoryToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && factoryToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty, 
                            factoryToCheck.InputStorage, 
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
                if (siteToCheck.DeconstructionMode) continue;

                if (siteToCheck.InputStorage.AreRequirementsMet == false
                    && siteToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && siteToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty, 
                            siteToCheck.InputStorage, 
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
            foreach (Storage storageToCheck in Storages)
            {
                if (storageToCheck.RequiresEmptying) continue;

                if (storageToCheck.UnreservedFreeSpace > 0)
                {
                    if (storageToEmpty.CanReserveResource(resourceID, character)
                        && storageToCheck.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            storageToEmpty, 
                            storageToCheck, 
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
                character.SetNewReservation(newReservation);
                return true;
            }
            else
            {
                newReservation.SourceStorage.RemoveResourceReservation(character);
                newReservation.TargetStorage.RemoveFreeSpaceReservation(character);
                return false;
            }
        }

        // Nie udało się nic znaleźć
        return false;
    }

    public Service GetClosestService(string need, Character character)
    {
        Service result = null;
        if (Services.ContainsKey(need))
        {
            float distance = Mathf.Infinity;
            for (int i = 0; i < Services[need].Count; i++)
            {
                Service service = Services[need][i];
                float newDistance = character.CurrentTile.DistanceTo(service.GetAccessTile());
                if (newDistance < distance)
                {
                    distance = newDistance;
                    result = service;
                }
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
