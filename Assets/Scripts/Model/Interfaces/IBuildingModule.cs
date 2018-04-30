using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IBuildingModule : IAccessible
{
    string DEBUG_GetSelectionText();
    Building Building { get; }
    bool HidesCharacter { get; }
    bool IsPreparingForDeconstruction();
    bool IsReadyForDeconstruction();
    void StartDeconstructionPreparation();
    void CancelDeconstructionPreparation();
    bool Halted { get; }
    void SetHalt(bool halt);
}
