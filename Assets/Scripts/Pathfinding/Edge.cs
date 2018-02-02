using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public class Edge
    {
        public Node Goal;
        public float Cost;
        public Edge(Node goal, float cost)
        {
            Goal = goal;
            Cost = cost;
        }        
    }
}
