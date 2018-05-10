using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class BT_Node
{
    public int ID { get; protected set; }
    public int ParentID { get; protected set; }
    public virtual bool IsAction { get { return true; } }

    public virtual bool CheckPrecondition(BT_AgentMemory am)
    {
        return true;
    }

    public virtual void Activate(BT_AgentMemory am)
    {
        // Debug.Log(ID + " - " + this.GetType().ToString() +  " - activate");
    }

    public virtual void Deactivate(BT_AgentMemory am)
    {
        // Debug.Log(ID + " - " + this.GetType().ToString() + " - deactivate");
    }

    public virtual BT_Result Tick(BT_AgentMemory am)
    {        
        return BT_Result.ERROR;
    }    

    public virtual void AssignID(int parentId, ref int idCounter, Dictionary<int, BT_Node> nodes)
    {        
        ParentID = parentId;
        ID = idCounter;
        idCounter++;

        nodes.Add(ID, this);
    }

    public static BT_Result TickChild(BT_Node child, BT_AgentMemory am)
    {
        if (child.CheckPrecondition(am) == false) return BT_Result.FAILURE;

        if (child.IsAction == false)
        {
            child.Activate(am);
            // Debug.Log(child.ID + " - " + child.GetType().ToString() + " - decider tick");
            BT_Result result = child.Tick(am);
            child.Deactivate(am);
            return result;
        }
        else
        {
            am.ActivateNode(child.ID);
            // Debug.Log(child.ID + " - " + child.GetType().ToString() + " - action tick");
            BT_Result result = child.Tick(am);
            if (result != BT_Result.RUNNING) am.DeactivateNode(child.ID);
            return result;
        }
    }
}    