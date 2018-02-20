using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{
    Empty,
    WalkableEmpty,
    Dirt,
    Rock    
}

public class Tile 
{
    public TilePosition Position;
    public int X { get { return Position.X; } }
    public int Y { get { return Position.Y; } }
    public int Height { get { return Position.Height; } }

    public float MovementCost = 2f;
    public TileType Type;

    public bool ReservedForAccess;

    public Building Building;

    World World;

    public Tile (int x, int y, int height, TileType type, World world)
    {
        Position = new TilePosition(x, y, height);
        World = world;
        Type = type;
    }

    public Tile GetNorthNeighbour()
    {
        return World.GetTileFromPosition(X, Y + 1, Height);
    }

    public Tile GetSouthNeighbour()
    {
        return World.GetTileFromPosition(X, Y - 1, Height);
    }

    public Tile GetEastNeighbour()
    {
        return World.GetTileFromPosition(X + 1, Y, Height);
    }

    public Tile GetWestNeighbour()
    {
        return World.GetTileFromPosition(X - 1, Y, Height);
    }

    public Tile GetNorthEastNeighbour()
    {
        return World.GetTileFromPosition(X + 1, Y + 1, Height);
    }

    public Tile GetSouthEastNeighbour()
    {
        return World.GetTileFromPosition(X + 1, Y - 1, Height);
    }

    public Tile GetSouthWestNeighbour()
    {
        return World.GetTileFromPosition(X - 1, Y - 1, Height);
    }

    public Tile GetNorthWestNeighbour()
    {
        return World.GetTileFromPosition(X - 1, Y + 1, Height);
    }

    public Tile GetUpperNeighbour()
    {
        return World.GetTileFromPosition(X, Y, Height + 1);
    }

    public Tile GetLowerNeighbour()
    {
        return World.GetTileFromPosition(X, Y, Height - 1);
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
        // CYy pola sąsiadują wYdłuż osi?
        if (Mathf.Abs(X - tile.X) + Mathf.Abs(Y - tile.Y) == 1)
        {
            return 1f;
        }

        //CYy pola sąsiadują po prYekątnych?
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
}


