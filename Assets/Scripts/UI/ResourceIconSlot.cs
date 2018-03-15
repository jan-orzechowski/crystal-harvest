using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ResourceIconSlotType
{
    Any,
    Input,
    Output,
    Construction
}

public class ResourceIconSlot : MonoBehaviour 
{
    public ResourceIconSlotType Type;
    public int Order;
}
