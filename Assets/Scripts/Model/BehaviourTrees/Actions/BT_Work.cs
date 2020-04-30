using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_Work : BT_Node
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace == null
            || (am.Workplace.GetAccessTile(am.UseWorkplaceSecondAccessTile) != am.Character.CurrentTile))
        {
            return BT_Result.ERROR;
        }
        else
        {
            bool result = am.Workplace.Work(am.DeltaTime, am.Character);

            if (result)
            {
                am.Character.DisplayObject.CharacterUsesModule(am.Workplace);
                return BT_Result.RUNNING;
            }
            else
            {
                return BT_Result.FAILURE;
            }
        }
    }
}
