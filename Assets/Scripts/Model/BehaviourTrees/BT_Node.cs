using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class BT_Node
{
    public int ID;

    public virtual BT_Result Tick(BT_AgentMemory am)
    {
        return BT_Result.ERROR;
    }       
}
