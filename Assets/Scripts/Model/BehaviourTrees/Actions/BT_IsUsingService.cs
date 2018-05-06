using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_IsUsingService : BT_Node
{
    public override bool IsAction { get { return false; } }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Character.State == CharacterState.UsingService)
        {
            am.Character.DisplayObject.CharacterUsesModule(am.Service);
            return BT_Result.SUCCESS;
        }
        else
        {
            return BT_Result.FAILURE;
        }
    }
}
