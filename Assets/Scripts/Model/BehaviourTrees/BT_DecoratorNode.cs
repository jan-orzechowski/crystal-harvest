using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_DecoratorNode : BT_Node
{
    public override bool Activates { get { return false; } }

    public BT_Node Child { get; protected set; }

    public BT_DecoratorNode(BT_Node child)
    {
        Child = child;
    }

    public override void AssignID(int parentId, ref int idCounter, Dictionary<int, BT_Node> nodes)
    {
        base.AssignID(parentId, ref idCounter, nodes);

        Child.AssignID(ID, ref idCounter, nodes);        
    }
}
