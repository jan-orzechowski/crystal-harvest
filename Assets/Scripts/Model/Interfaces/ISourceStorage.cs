using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISourceStorage
{ 
    Tile GetAccessTile();
    Rotation GetAccessTileRotation();
    bool TransferFromStorage(int resourceID, Character character);
    bool ReserveResource(int resourceID, Character character);
    bool RemoveResourceReservation(Character character);
    string GetSelectionText();
}
