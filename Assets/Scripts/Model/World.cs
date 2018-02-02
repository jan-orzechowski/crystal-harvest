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

    List<Building> buildings;
    List<Character> characters;

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
                if (x < 6 && y < 6) // piach
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

        buildings = new List<Building>(1024);
        characters = new List<Character>(32);

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

        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].UpdateCharacter(deltaTime);
        }
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].UpdateBuilding(deltaTime);
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
            characters.Add(c);

            GameManager.Instance.GenerateDisplayForCharacter(c);
            Debug.Log("Dodano postać: " + c.CurrentTile.Position.ToString());
            return true;
        }
        return false;
    }

    public bool IsValidBuildingPosition(Tile tile, BuildingPrototype prototype)
    {
        bool result =   (tile != null &&
                        tile.Building == null &&
                        tile.Type != TileType.Empty);
        return result;
    }

    public bool IsValidBuildingPosition(List<Tile> tiles, BuildingPrototype prototype)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (IsValidBuildingPosition(tiles[i], prototype))
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public List<Tile> MapPrototypeToWorld(TilePosition origin, Rotation rotation, BuildingPrototype prototype)
    {
        List<Tile> tiles = new List<Tile>();
        for (int i = 0; i < prototype.NormalizedTilePositions.Count; i++)
        {
            TilePosition tp = prototype.NormalizedTilePositions[i];

            Tile tile = null;

            if (rotation == Rotation.N)
            {
                tile = GetTileFromPosition(new TilePosition(
                origin.X + tp.X,
                origin.Y + tp.Y,
                origin.Height + tp.Height));
            }
            else if (rotation == Rotation.E)
            {
                tile = GetTileFromPosition(new TilePosition(
                origin.X + tp.Y,
                origin.Y - tp.X, //
                origin.Height + tp.Height));
            }
            else if (rotation == Rotation.S)
            {
                tile = GetTileFromPosition(new TilePosition(
                origin.X - tp.X,
                origin.Y - tp.Y,
                origin.Height + tp.Height));
            }
            else if (rotation == Rotation.W)
            {
                tile = GetTileFromPosition(new TilePosition(
                origin.X - tp.Y,
                origin.Y + tp.X,
                origin.Height + tp.Height));
            }

            if (tile == null)
            {
                return null;
            }

            tiles.Add(tile);
        }
        return tiles;
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
        List<Tile> validTiles = MapPrototypeToWorld(origin, rotation, prototype);
        if (validTiles == null ||
            IsValidBuildingPosition(validTiles, prototype) == false
            )
        {
            Debug.Log("Nie udało się wybudować budynku: pola są niedostępne.");
            return;
        }
        else
        {
            Building b = new Building(prototype.Type, rotation, new List<Tile>(validTiles));
            buildings.Add(b);
            for (int i = 0; i < validTiles.Count; i++)
            {
                validTiles[i].Building = b;
                validTiles[i].MovementCost = 0f;
                modifiedTiles.Add(validTiles[i]);
            }
            GameManager.Instance.ShowBuilding(b, rotation);

            mapChangedThisFrame = true;

            Debug.Log("Dodano budynek: " + b.Tiles[0].Position.ToString());
        }       
    }

    public bool DeleteBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            for (int i = 0; i < building.Tiles.Count; i++)
            {
                building.Tiles[i].Building = null;
                building.Tiles[i].MovementCost = 1f;
                modifiedTiles.Add(building.Tiles[i]);
            }
            GameManager.Instance.RemoveDisplayForBuilding(building);
            buildings.Remove(building);

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
        buildingPrototypes.Add(bp);

    }
}
