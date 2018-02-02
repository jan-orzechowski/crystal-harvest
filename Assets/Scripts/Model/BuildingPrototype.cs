using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingPrototype
{    
    public string Type;
    public string ModelName;
    public bool CanBeBuiltOnSand; 
    public bool CanBeBuiltOnRock;
    public bool CanBeBuiltOnPlatform;
    public bool AllowRotation;
    public List<TilePosition> NormalizedTilePositions;
}
