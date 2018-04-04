using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IBuildingModule
{
    string GetSelectionText();
    bool HidesCharacter { get; }
}
