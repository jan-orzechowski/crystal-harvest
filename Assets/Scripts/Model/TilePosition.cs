using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct TilePosition : IEquatable<TilePosition>
{
    public int X;
    public int Y;
    public int Height;

    public TilePosition(int x, int y, int height)
    {
        X = x;
        Y = y;
        Height = height;
    }

    public static bool operator == (TilePosition a, TilePosition b)
    {
        return (a.X == b.X && a.Y == b.Y && a.Height == b.Height);
    }

    public static bool operator != (TilePosition a, TilePosition b)
    {
        return !(a==b);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = Height * 26681 + X * 17659 + Y * 99371;
            return hash.GetHashCode();
        }
    }
    public override bool Equals(object o)
    {
        if (o == null) return false;
        if (o.GetType() != this.GetType()) return false;
        return ((TilePosition)o == this);
    }
    public bool Equals(TilePosition tp)
    {
        return (tp == this);
    }
    public override string ToString()
    {
        return "(" + X + "," + Y + "," + Height + ")";
    }
}
