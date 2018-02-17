using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ITargetStorage 
{
    Tile GetAccessTile();
    Rotation GetAccessTileRotation();
    bool TransferToStorage(int resourceID, Character character);
    bool ReserveFreeSpace(int resourceID, Character character);
    string GetSelectionText();
}
