using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindNewStorageForResource : BT_Node 
{
    static float timeoutAfterNullResults = 1f;

    BT_FindNewStorageForResourceSubnode subnode;

    class BT_FindStorageForResourceData
    {
        public ResourceReservation PotentialReservation;
    }

    public BT_FindNewStorageForResource()
    {
        subnode = new BT_FindNewStorageForResourceSubnode();
    }

    public override void Activate(BT_AgentMemory am)
    {
        am.SetObject(ID, new BT_FindStorageForResourceData());
    }

    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.HasTimerElapsed(ID));
    }

    public override void Deactivate(BT_AgentMemory am)
    {
        BT_FindStorageForResourceData data = am.GetObject(ID) as BT_FindStorageForResourceData;
        if (data != null && data.PotentialReservation != null)
        {
            data.PotentialReservation.TargetStorage.RemoveFreeSpaceReservation(am.Character);
        }
        am.SetObject(ID, null);
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        BT_FindStorageForResourceData data = am.GetObject(ID) as BT_FindStorageForResourceData;
        if (data == null) return BT_Result.FAILURE; 

        data.PotentialReservation = GameManager.Instance.World.GetReservationToStoreCurrentResource(am.Character);

        if (data.PotentialReservation == null)
        {
            am.SetTimer(ID, timeoutAfterNullResults);
            return BT_Result.FAILURE;
        }

        BT_Result result = TickChild(subnode, am);

        if (result == BT_Result.SUCCESS)
        {
            if (data.PotentialReservation.TargetStorage.ReserveFreeSpace(am.Character.Resource, am.Character))
            {
                am.Character.SetNewReservation(data.PotentialReservation);
                data.PotentialReservation = null;
                return BT_Result.SUCCESS;
            }
            else
            {
                return BT_Result.FAILURE;
            }            
        }
        else
        {
            return result;
        }
    }

    class BT_FindNewStorageForResourceSubnode : BT_Find
    {
        public override bool IsSearchNeededCondition(BT_AgentMemory am)
        {
            return true;
        }

        public override IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile)
        {
            BT_FindStorageForResourceData data = am.GetObject(ParentID) as BT_FindStorageForResourceData;
            return data.PotentialReservation.TargetStorage;
        }

        public override void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed)
        {
            return;
        }
    }

    public override void AssignID(int parentId, ref int idCounter, Dictionary<int, BT_Node> nodes)
    {
        base.AssignID(parentId, ref idCounter, nodes);
        subnode.AssignID(ID, ref idCounter, nodes);
    }
}
