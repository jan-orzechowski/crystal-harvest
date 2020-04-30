using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindTransportJob : BT_Node 
{
    static float timeoutAfterFailure = 1f;

    BT_FindTargetStorage findTargetStorageNode;
    BT_FindSourceStorage findSourceStorageNode;

    class BT_FindTransportJobNodeData
    {
        public ResourceReservation PotentialReservation;
        public bool TargetStorageChecked;
        public bool SourceStorageChecked;
    }

    public BT_FindTransportJob()
    {
        findTargetStorageNode = new BT_FindTargetStorage();
        findSourceStorageNode = new BT_FindSourceStorage();
    }

    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.Character.Reservation == null
                && am.HasTimerElapsed(ID));
    }

    public override void Activate(BT_AgentMemory am)
    {
        am.SetObject(ID, new BT_FindTransportJobNodeData());     
    }

    public override void Deactivate(BT_AgentMemory am)
    {
        BT_FindTransportJobNodeData data = am.GetObject(ID) as BT_FindTransportJobNodeData;

        if (data == null) return;

        // Jeśli oba magazyny nie zostały sprawdzone, odwołanie rezerwacji
        if (data.TargetStorageChecked == false || data.SourceStorageChecked == false)
        {
            if (data.PotentialReservation != null)
            {
                data.PotentialReservation.SourceStorage.RemoveResourceReservation(am.Character);
                data.PotentialReservation.TargetStorage.RemoveFreeSpaceReservation(am.Character);
            }      
        }

        am.SetObject(ID, null);
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        BT_FindTransportJobNodeData data = am.GetObject(ID) as BT_FindTransportJobNodeData;
        if (data == null) return BT_Result.FAILURE;

        World world = GameManager.Instance.World;

        if (data.PotentialReservation == null)
        {
            ResourceReservation newReservation = null;

            newReservation = world.GetReservationForFillingInput(am.Character);

            if (newReservation == null) newReservation = world.GetReservationForEmptying(am.Character);

            if (newReservation != null)
            {
                data.PotentialReservation = newReservation;
                data.TargetStorageChecked = false;
                data.SourceStorageChecked = false;
            }
            else
            {
                am.SetTimer(ID, timeoutAfterFailure);
                return BT_Result.FAILURE;
            }

            return BT_Result.RUNNING;
        }

        if (data.TargetStorageChecked == false)
        {
            BT_Result result = TickChild(findTargetStorageNode, am);

            if (result == BT_Result.SUCCESS)
            {
                data.TargetStorageChecked = true;
                return BT_Result.RUNNING;
            }
            else if (result == BT_Result.RUNNING)
            {
                return BT_Result.RUNNING;
            }
            else
            {
                am.SetTimer(ID, timeoutAfterFailure);
                return BT_Result.FAILURE;
            }
        }
        else
        {
            BT_Result result = TickChild(findSourceStorageNode, am);

            if (result == BT_Result.SUCCESS)
            {
                data.SourceStorageChecked = true;

                if (am.Character.SetNewReservation(data.PotentialReservation))                                         
                {
                    am.SetObject(ID, null);
                    return BT_Result.SUCCESS;
                }
                else
                {
                    data.PotentialReservation.SourceStorage.RemoveResourceReservation(am.Character);
                    data.PotentialReservation.TargetStorage.RemoveFreeSpaceReservation(am.Character);

                    am.SetTimer(ID, timeoutAfterFailure);
                    return BT_Result.FAILURE;
                }
            }
            else if (result == BT_Result.RUNNING)
            {
                return BT_Result.RUNNING;
            }
            else
            {
                am.SetTimer(ID, timeoutAfterFailure);
                return BT_Result.FAILURE;
            }
        }         
    }

    class BT_FindSourceStorage : BT_Find
    {
        public override bool IsSearchNeededCondition(BT_AgentMemory am)
        {
            return true;
        }

        public override IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile)
        {
            BT_FindTransportJobNodeData data = am.GetObject(ParentID) as BT_FindTransportJobNodeData;
            if (am.Character.AreBothAccessTilesMarkedAsInaccessbile(data.PotentialReservation.SourceStorage))
            {
                return null;
            }
            return data.PotentialReservation.SourceStorage;
        }

        public override void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed)
        {
            BT_FindTransportJobNodeData data = am.GetObject(ParentID) as BT_FindTransportJobNodeData;
            data.PotentialReservation.UseSourceStorageSecondAccessTile = secondAccessTileUsed;
        }
    }

    class BT_FindTargetStorage : BT_Find
    {
        public override bool IsSearchNeededCondition(BT_AgentMemory am)
        {
            return true;
        }

        public override IAccessible GetPotentialDestination(BT_AgentMemory am, bool secondAccessTile)
        {
            BT_FindTransportJobNodeData data = am.GetObject(ParentID) as BT_FindTransportJobNodeData;
            if (am.Character.AreBothAccessTilesMarkedAsInaccessbile(data.PotentialReservation.TargetStorage))
            {
                return null;
            }
            return data.PotentialReservation.TargetStorage;
        }

        public override void Found(BT_AgentMemory am, IAccessible matchingCandidate, bool secondAccessTileUsed)
        {
            BT_FindTransportJobNodeData data = am.GetObject(ParentID) as BT_FindTransportJobNodeData;
            data.PotentialReservation.UseTargetStorageSecondAccessTile = secondAccessTileUsed;
        }
    }

    public override void AssignID(int parentId, ref int idCounter, Dictionary<int, BT_Node> nodes)
    {
        base.AssignID(parentId, ref idCounter, nodes);
        findTargetStorageNode.AssignID(ID, ref idCounter, nodes);
        findSourceStorageNode.AssignID(ID, ref idCounter, nodes);
    }
}
