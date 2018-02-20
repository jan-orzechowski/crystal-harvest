using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public class Node
    {
        public Tile Tile;

        public int X { get { return Tile.X; } }
        public int Y { get { return Tile.Y; } }

        public int Height { get { return Tile.Height; } }

        public List<Edge> Edges;

        public Node(Tile tile)
        {
            Tile = tile;
        }
    }
}
