using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAccessible
{
    Tile GetAccessTile(bool secondTile = false);
    Rotation GetAccessTileRotation(bool secondTile = false);    
}
