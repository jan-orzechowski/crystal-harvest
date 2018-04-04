using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_StartUsingService : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Service == null || am.Service.GetAccessTile() != am.Character.CurrentTile)
        {
            return BT_Result.ERROR;
        }
        else
        {
            bool result = am.Service.StartService(am.Character);
            if (result)
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
}
