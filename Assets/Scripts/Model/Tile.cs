using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{
    Empty,
    WalkableEmpty,
    Sand,
    Rock    
}

public class Tile 
{
    public TilePosition Position;
    public int X { get { return Position.X; } }
    public int Y { get { return Position.Y; } }
    public int Height { get { return Position.Height; } }

    public float MovementCost;
    public TileType Type;

    public bool ReservedForAccess;

    public bool AllowDiagonal = true;

    static World world;

    public Tile (int x, int y, int height, TileType type, float movementCost)
    {
        Position = new TilePosition(x, y, height);
        Type = type;
        MovementCost = movementCost;
    }

    public static bool CheckPassability(Tile t)
    {
        return (t != null && t.Type != TileType.Empty && t.MovementCost != 0);
    }

    public static bool CheckDiagonalPassability(Tile t)
    {
        return CheckPassability(t) && t.AllowDiagonal;
    }

    public Tile GetNorthNeighbour()
    {
        return world.GetTileFromPosition(X, Y + 1, Height);
    }

    public Tile GetSouthNeighbour()
    {
        return world.GetTileFromPosition(X, Y - 1, Height);
    }

    public Tile GetEastNeighbour()
    {
        return world.GetTileFromPosition(X + 1, Y, Height);
    }

    public Tile GetWestNeighbour()
    {
        return world.GetTileFromPosition(X - 1, Y, Height);
    }

    public Tile GetNorthEastNeighbour()
    {
        return world.GetTileFromPosition(X + 1, Y + 1, Height);
    }

    public Tile GetSouthEastNeighbour()
    {
        return world.GetTileFromPosition(X + 1, Y - 1, Height);
    }

    public Tile GetSouthWestNeighbour()
    {
        return world.GetTileFromPosition(X - 1, Y - 1, Height);
    }

    public Tile GetNorthWestNeighbour()
    {
        return world.GetTileFromPosition(X - 1, Y + 1, Height);
    }

    public Tile GetUpperNeighbour()
    {
        return world.GetTileFromPosition(X, Y, Height + 1);
    }

    public Tile GetLowerNeighbour()
    {
        return world.GetTileFromPosition(X, Y, Height - 1);
    }

    public bool IsNeighbour(Tile tile, bool diagonal)
    {
        if (Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1)
        {
            return true;
        }
        else if (diagonal == true && (Mathf.Abs(X - tile.X) == 1 && Mathf.Abs(Y - tile.Y) == 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float DistanceTo(Tile tile)
    {
        // Czy pola sąsiadują wYdłuż osi?
        if (Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1)
        {
            return 1f;
        }

        // Czy pola sąsiadują po prYekątnych?
        if (Mathf.Abs(X - tile.X) == 1 && Mathf.Abs(Y - tile.Y) == 1)
        {
            return 1.41421356237f;
        }

        // Gdy pola nie sąsiadują
        return Mathf.Sqrt(
            Mathf.Pow(X - tile.X, 2) +
            Mathf.Pow(Y - tile.Y, 2)
            );
    }

    public Tile[] GetNeighbours(bool diagonal = false)
    {
        if (diagonal == true)
        {
            return new Tile[]
            {
                GetNorthNeighbour(),
                GetNorthWestNeighbour(),
                GetEastNeighbour(),
                GetSouthEastNeighbour(),
                GetSouthNeighbour(),
                GetSouthWestNeighbour(),
                GetWestNeighbour(),
                GetNorthWestNeighbour()
            };
        }
        else
        {
            return new Tile[]
            {
                GetNorthNeighbour(),
                GetEastNeighbour(),
                GetSouthNeighbour(),
                GetWestNeighbour()
            };
        }
    }

    public Tile[] GetUpperNeighbours()
    {
        Tile[] result = new Tile[4];
        if (GetNorthNeighbour() != null) result[0] = GetNorthNeighbour().GetUpperNeighbour();
        if (GetEastNeighbour() != null)  result[1] = GetEastNeighbour().GetUpperNeighbour();
        if (GetSouthNeighbour() != null) result[2] = GetSouthNeighbour().GetUpperNeighbour();
        if (GetWestNeighbour() != null)  result[3] = GetWestNeighbour().GetUpperNeighbour();
        return result;
    }

    public static void SetWorldForTiles(World world)
    {
        Tile.world = world;
    }

    public override string ToString()
    {
        return Position.ToString();
    }
}


