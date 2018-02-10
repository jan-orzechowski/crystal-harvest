using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IStorage
{
    Tile GetAccessTile();
    bool TransferFromStorage(int resourceID, Character character);
    bool TransferToStorage(int resourceID, Character character);
    bool ReserveResource(int resourceID, Character character);
    bool ReserveFreeSpace(int resourceID, Character character);
    string GetSelectionText();    
}
