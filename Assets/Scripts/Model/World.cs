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

    public Dictionary<int, int> AllResources { get; protected set; }

    public int ResourceToGather { get; protected set; }
    public int AmountToGather { get; protected set; }


    public Pathfinder Pathfinder { get; protected set; }

    BT_Tree humanBehaviourTree;
    BT_Tree robotBehaviourTree;

    List<Tile> modifiedTiles;
    bool mapChangedThisFrame;

    public World(int width, int length)
    {
        XSize = width;
        YSize = length;
        Height = 2;
               
        Tiles = new Tile[XSize, YSize, Height];

        Debug.Log("Stworzono mapę posiadającą " + XSize * YSize * Height + " pól.");
      
        // Generowanie losowej mapy
        Tile newTile;
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (x < 15 && y < 15) // piach
                {
                    newTile = new Tile(x, y, 0, TileType.Sand, this); // piach
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, this); // puste
                    Tiles[x, y, 1] = newTile;
                    continue;
                }

                if (y > 17) // skały
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, this); // puste
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock, this); // skały
                    Tiles[x, y, 1] = newTile;
                    continue;
                }

                if (UnityEngine.Random.Range(0,3) == 0) // skały
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, this); // puste
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock, this); // skały
                    Tiles[x, y, 1] = newTile;
                }
                else // piach
                {
                    newTile = new Tile(x, y, 0, TileType.Sand, this); // piach
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, this); // puste
                    Tiles[x, y, 1] = newTile;
                }                
            }
        }

        Buildings = new List<Building>(1024);
        Characters = new List<Character>(32);

        buildingPrototypes = StaticData.LoadPrototypes();
        ResourcesInfo = StaticData.LoadResources();

        AllResources = new Dictionary<int, int>();
        foreach (int resourceID in ResourcesInfo.Keys)
        {
            AllResources.Add(resourceID, 0);
        }

        ResourceToGather = 0;
        AmountToGather = 100;

        Pathfinder = new Pathfinder(this);
        modifiedTiles = new List<Tile>();

        humanBehaviourTree = new BT_Tree();
        humanBehaviourTree.DEBUG_LoadTestTree();

        robotBehaviourTree = new BT_Tree();
        robotBehaviourTree.DEBUG_LoadTestTree();

        Storages = new List<Storage>();
        Factories = new List<Factory>();
        Services = new Dictionary<string, List<Service>>();
        Stairs = new List<Building>();
        Platforms = new Dictionary<Tile, Building>();        

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
        for (int i = 0; i < Buildings.Count; i++)
        {
            Buildings[i].UpdateBuilding(deltaTime);
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
    }

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

#if DEBUG
        if (Tiles[tilePosition.X, tilePosition.Y, tilePosition.Height] == null)
        {
            return null;
        }
#endif

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

    public bool CreateNewCharacter(TilePosition tilePosition, bool isRobot)
    {
        Tile t = GetTileFromPosition(tilePosition);

        if (Tile.CheckPassability(t))
        {
            Character c;
            if (isRobot)
            {
                c = new Character("Robot", t, robotBehaviourTree);
            }
            else
            {
                c = new Character("Wiesław", t, humanBehaviourTree);
            }
            Characters.Add(c);

            GameManager.Instance.GenerateDisplayForCharacter(c);
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

    public bool IsValidBuildingPosition(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        List<Tile> tilesToCheck = MapPrototypeToWorld(origin, rotation, prototype);

        for (int i = 0; i < tilesToCheck.Count; i++)
        {
            Tile tile = tilesToCheck[i];
            if (Tile.CheckPassability(tile)
                && ! (prototype.CanBeBuiltOnSand == false && tile.Type == TileType.Sand)
                && ! (prototype.CanBeBuiltOnRock == false && tile.Type == TileType.Rock)
                && ! (prototype.CanBeBuiltOnPlatform == false && tile.Type == TileType.WalkableEmpty)
                && ! (prototype.AllowToBuildOnTop == false && Platforms.ContainsKey(tile))
                && (((IsCharacterOnTile(tile) == false && tile.ReservedForAccess != true) || prototype.MovementCost != 0f))
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

            for (int i = 0; i < tilesForBuilding.Count; i++)
            {
                //tilesForBuilding[i].Building = newBuilding;
                tilesForBuilding[i].MovementCost = prototype.MovementCost;
                modifiedTiles.Add(tilesForBuilding[i]);

                if (prototype.DisallowDiagonalMovement) tilesForBuilding[i].AllowDiagonal = false;
            }

            if (prototype.WalkableOnTop)
            {
                for (int i = 0; i < tilesForBuilding.Count; i++)
                {
                    Tile tileOnTop = tilesForBuilding[i].GetUpperNeighbour();
                    if(tileOnTop != null)
                    {
                        tileOnTop.Type = TileType.WalkableEmpty;
                        tileOnTop.MovementCost = prototype.MovementCostOnTop;
                        modifiedTiles.Add(tileOnTop);
                    }
                }
            }

            mapChangedThisFrame = true;
            
            ConstructionSite newConstructionSite = new ConstructionSite(newBuilding, prototype);
            ConstructionSites.Add(newConstructionSite);

            GameManager.Instance.ShowConstructionSite(newConstructionSite);

            Debug.Log("Dodano budynek: " + newBuilding.Tiles[0].Position.ToString());

            return newConstructionSite;
        }       
    }

    public void InstantBuild(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        ConstructionSite site = PlaceNewConstructionSite(origin, rotation, prototype);
        if (site != null)
        {
            Debug.Log("InstantBuild");
            FinishConstruction(site);
        }
    }

    public bool MarkBuildingToDeconstruction(Building building)
    {

        return false;
    }

    bool DeleteBuilding(Building building)
    {
        if (Buildings.Contains(building))
        {
            CancelBuildingReservationForAccess(building);

            for (int i = 0; i < building.Tiles.Count; i++)
            {
                building.Tiles[i].MovementCost = 1f;
                if (Platforms.ContainsKey(building.Tiles[i]) && building == Platforms[building.Tiles[i]])
                {
                    building.Tiles[i].AllowDiagonal = true;
                }
               
                modifiedTiles.Add(building.Tiles[i]);
            }

            BuildingPrototype prototype = GetBuildingPrototype(building.Type);
            if (prototype != null && prototype.WalkableOnTop)
            {
                for (int i = 0; i < building.Tiles.Count; i++)
                {
                    Tile tileOnTop = building.Tiles[i].GetUpperNeighbour();
                    if (tileOnTop != null)
                    {
                        tileOnTop.Type = TileType.Empty;
                        tileOnTop.MovementCost = 0f;
                        modifiedTiles.Add(tileOnTop);
                    }
                }
            }

            if (prototype.Type == "Stairs")
            {
                Stairs.Remove(building);
            }
            else if (prototype.Type == "Platform")
            {
                if (Platforms.ContainsKey(building.Tiles[0]))
                    Platforms.Remove(building.Tiles[0]);
            }

            GameManager.Instance.RemoveDisplayForBuilding(building);
            Buildings.Remove(building);

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

            mapChangedThisFrame = true;

            Debug.Log("Usunięto budynek: " + building.Tiles[0].Position.ToString());
            return true;
        }
        else
        {
            Debug.Log("Próbujemy usunąć budynek, którego nie ma na liście!");
            return false;
        }
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

    public void FinishConstruction(ConstructionSite site)
    {
        if (ConstructionSites.Contains(site) == false || site.Building == null)
        {
            Debug.Log("Próba ukończenia konstrukcji przez niezarejestrowany plac budowy");
            return;
        }

        Building buildingToConstruct = site.Building;

        buildingToConstruct.LoadDataForFinishedBuilding();

        if(buildingToConstruct.Type == "Stairs")
        {
            Stairs.Add(buildingToConstruct);
        }
        else if (buildingToConstruct.Type == "Platform")
        {
            Platforms.Add(buildingToConstruct.Tiles[0], buildingToConstruct);
        }

        IBuildingModule module = buildingToConstruct.Module;
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

        GameManager.Instance.RemoveConstructionSiteDisplay(site);   
       
        TilePosition positionForBuildingDisplay = site.Building.Tiles[0].Position - 
                MapNormalizedPositionToWorld(site.Prototype.NormalizedTilePositions[0], 
                                             new TilePosition(0,0,0), 
                                             buildingToConstruct.Rotation);

        site.Building = null;
        ConstructionSites.Remove(site);

        GameManager.Instance.ShowBuilding(buildingToConstruct, positionForBuildingDisplay);
    }

    public void FinishDeconstruction(ConstructionSite site)
    {
        if (ConstructionSites.Remove(site) == false || site.Building == null)
        {
            Debug.Log("Próba ukończenia dekonstrukcji przez niezarejestrowany plac budowy");
            return;
        }

        Building buildingToDeconstruct = site.Building;

        site.Building = null;
        ConstructionSites.Remove(site);

        //DeleteBuilding(buildingToDeconstruct);
    }
    
    public bool GetReservationForFillingInput(Character character)
    {
        foreach (Factory factory in Factories)
        {
            if (GetReservationForFillingInput(character, factory))
            {
                return true;
            }
        }

        foreach (ConstructionSite site in ConstructionSites)
        {
            if (GetReservationForFillingInput(character, site))
            {
                return true;
            }
        }

        return false;
    }

    public bool GetReservationForFillingInput(Character character, IWorkplace workplaceToFill)
    {
        return GetReservationForFillingInput(character, workplaceToFill.InputStorage);
    }

    public bool GetReservationForFillingInput(Character character, StorageToFill storageToFill)
    {
        if (character.Reservation != null || storageToFill.IsFilled)
        {
            return false;
        }

        ResourceReservation newReservation = null;
        
        foreach (int resourceID in storageToFill.MissingResources.Keys)
        {
            // Czy nie ma potrzebnych zasobów w jakiejś fabryce do opróżnienia?
            foreach (Factory factoryToCheck in Factories)
            {
                if (factoryToCheck.OutputStorage.ResourcesToRemove.ContainsKey(resourceID))
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

            if(newReservation != null)
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

    public bool GetReservationForHandlingOutput(Character character)
    {
        foreach (Factory factory in Factories)
        {
            if (GetReservationForHandlingOutput(character, factory))
            {
                return true;
            }
        }
        return false;
    }

    public bool GetReservationForHandlingOutput(Character character, IWorkplace workplaceWithOutput)
    {
        if (character.Reservation != null || workplaceWithOutput.OutputStorage.IsEmpty)
        {
            return false;
        }

        ResourceReservation newReservation = null;

        foreach (int resourceID in workplaceWithOutput.OutputStorage.ResourcesToRemove.Keys)
        {
            // Czy jakaś fabryka nie potrzebuje tego zasobu?
            foreach (Factory factoryToCheck in Factories)
            {
                if (factoryToCheck.InputStorage.IsFilled == false
                    && factoryToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (workplaceWithOutput.OutputStorage.CanReserveResource(resourceID, character)
                        && factoryToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            workplaceWithOutput.OutputStorage, 
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

                if (siteToCheck.InputStorage.IsFilled == false
                    && siteToCheck.InputStorage.MissingResources.ContainsKey(resourceID))
                {
                    if (workplaceWithOutput.OutputStorage.CanReserveResource(resourceID, character)
                        && siteToCheck.InputStorage.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            workplaceWithOutput.OutputStorage, 
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
                if (storageToCheck.UnreservedFreeSpace > 0)
                {
                    if (workplaceWithOutput.OutputStorage.CanReserveResource(resourceID, character)
                        && storageToCheck.CanReserveFreeSpace(resourceID, character))
                    {
                        newReservation = new ResourceReservation(
                            workplaceWithOutput.OutputStorage, 
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
}
