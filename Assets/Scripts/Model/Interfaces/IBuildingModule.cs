using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IBuildingModule
{
    string DEBUG_GetSelectionText();
    bool HidesCharacter { get; }
    bool IsReadyForDeconstruction();
    void StartDeconstructionPreparation();
    void CancelDeconstructionPreparation();
}
