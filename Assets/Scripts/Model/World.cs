using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public enum Rotation
{
    N,
    E,
    S,
    W
}

public class World 
{
    public int XSize { get; protected set; }
    public int YSize { get; protected set; } 
    public int Height { get; protected set; }

    public Tile[,,] Tiles { get; protected set; }

    public List<Building> Buildings { get; protected set; }
    public List<Character> Characters { get; protected set; }

    List<BuildingPrototype> buildingPrototypes;

    public Pathfinder Pathfinder { get; protected set; }

    BT_Tree humanBehaviourTree;

    List<Tile> modifiedTiles;
    bool mapChangedThisFrame;

    public World(int width, int length)
    {
        XSize = width;
        YSize = length;
        Height = 3;
               
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
                    newTile = new Tile(x, y, 0, TileType.Dirt, this); // piach
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, this); // puste
                    Tiles[x, y, 1] = newTile;
                    newTile = new Tile(x, y, 2, TileType.Empty, this); // puste
                    Tiles[x, y, 2] = newTile;
                    continue;
                }

                if (UnityEngine.Random.Range(0,3) == 0) // skały
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, this); // puste
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock, this); // skały
                    Tiles[x, y, 1] = newTile;
                    newTile = new Tile(x, y, 2, TileType.Empty, this); // puste
                    Tiles[x, y, 2] = newTile;
                }
                else // piach
                {
                    newTile = new Tile(x, y, 0, TileType.Dirt, this); // piach
                    Tiles[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, this); // puste
                    Tiles[x, y, 1] = newTile;
                    newTile = new Tile(x, y, 2, TileType.Empty, this); // puste
                    Tiles[x, y, 2] = newTile;
                }                
            }
        }

        Buildings = new List<Building>(1024);
        Characters = new List<Character>(32);

        DEBUG_LoadPrototypes();

        Pathfinder = new Pathfinder(this);
        modifiedTiles = new List<Tile>();

        humanBehaviourTree = new BT_Tree();
        humanBehaviourTree.DEBUG_LoadTestTree();

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

    public bool CreateNewCharacter(TilePosition tilePosition)
    {
        Tile t = GetTileFromPosition(tilePosition);

        if (t != null && t.Building == null)
        {
            Character c = new Character("Wiesław", t, humanBehaviourTree);
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
            if (tile != null
                && tile.Building == null
                && tile.Type != TileType.Empty
                && (IsCharacterOnTile(tile) == false || prototype.MovementCost != 0f)
                && (prototype.DoesNotBlockAccess || tile.ReservedForAccess != true)
                )
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        if (prototype.HasAccessTile)
        {
            Tile accessTile = MapPrototypeAccessTileToWorld(origin, rotation, prototype);

            if (accessTile != null
                && (accessTile.Building == null || accessTile.Building.DoesNotBlockAccess)
                && accessTile.Type != TileType.Empty
                && accessTile.MovementCost > 0f
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public Tile MapPrototypeAccessTileToWorld(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        if (prototype.HasAccessTile)
        {
            return GetTileFromPosition(MapNormalizedPositionToWorld(
                        prototype.NormalizedAccessTilePosition,
                        origin, rotation));
        }
        else
        {
            return null;
        }
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

    TilePosition MapNormalizedPositionToWorld(TilePosition normalizedPosition, TilePosition origin, Rotation rotation)
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

    public void PlaceNewBuilding(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        if (IsValidBuildingPosition(origin, rotation, prototype) == false)
        {
            Debug.Log("Nie udało się wybudować budynku: lokacja jest nieodpowiednia.");
            return;
        }
        else
        {
            List<Tile> tilesForBuilding = MapPrototypeToWorld(origin, rotation, prototype);
            Tile accessTile = MapPrototypeAccessTileToWorld(origin, rotation, prototype);

            Building b = new Building(prototype, rotation, tilesForBuilding, accessTile);
            Buildings.Add(b);

            for (int i = 0; i < tilesForBuilding.Count; i++)
            {
                tilesForBuilding[i].Building = b;
                tilesForBuilding[i].MovementCost = prototype.MovementCost;
                modifiedTiles.Add(tilesForBuilding[i]);
            }
            mapChangedThisFrame = true;

            if (accessTile != null) { accessTile.ReservedForAccess = true; }

            GameManager.Instance.ShowBuilding(b, rotation);
            
            Debug.Log("Dodano budynek: " + b.Tiles[0].Position.ToString());
        }       
    }

    public bool DeleteBuilding(Building building)
    {
        if (Buildings.Contains(building))
        {
            CancelReservationForAccess(building);

            for (int i = 0; i < building.Tiles.Count; i++)
            {
                building.Tiles[i].Building = null;
                building.Tiles[i].MovementCost = 1f;
                modifiedTiles.Add(building.Tiles[i]);
            }
            GameManager.Instance.RemoveDisplayForBuilding(building);
            Buildings.Remove(building);

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

    void CancelReservationForAccess(Building building)
    {
        if (building.AccessTile == null) return;

        for (int i = 0; i < Buildings.Count; i++)
        {
            if(Buildings[i].AccessTile != null 
                && Buildings[i].AccessTile == building.AccessTile 
                && Buildings[i] != building)
            {
                return;
            }
        }
        building.AccessTile.ReservedForAccess = false;
    }

    void DEBUG_LoadPrototypes()
    {
        buildingPrototypes = new List<BuildingPrototype>();

        BuildingPrototype bp;

        bp = new BuildingPrototype();
        bp.Type = "Debug1";
        bp.ModelName = "Debug1";
        bp.CanBeBuiltOnPlatform = false;
        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnSand = true;
        bp.AllowRotation = false;
        bp.DoesNotBlockAccess = true;
        bp.MovementCost = 0.5f;
        bp.HasAccessTile = false;
        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(0,0,0),
        };
        buildingPrototypes.Add(bp);
        


        bp = new BuildingPrototype();
        bp.Type = "Debug2";
        bp.ModelName = "Debug2";
        bp.CanBeBuiltOnPlatform = false;
        bp.CanBeBuiltOnRock = true;
        bp.CanBeBuiltOnSand = true;
        bp.AllowRotation = true;
        bp.NormalizedTilePositions = new List<TilePosition>()
        {
            new TilePosition(0,0,0),
            new TilePosition(0,1,0),
        };
        bp.HasAccessTile = true;
        bp.MovementCost = 0f;
        bp.NormalizedAccessTilePosition = new TilePosition(1, 1, 0);
        bp.NormalizedAccessTileRotation = Rotation.W;

        buildingPrototypes.Add(bp);

    }
}
