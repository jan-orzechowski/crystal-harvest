using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISelectable
{
    SelectableDisplayObject GetDisplayObject();
    string GetSelectionText();
}
