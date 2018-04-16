using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_Die : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        GameManager.Instance.World.MarkCharacterForDeletion(am.Character);
        return BT_Result.SUCCESS;
    }
}
