using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_CompositeNode : BT_Node
{
    public override bool IsAction { get { return false; } }

    public List<BT_Node> Children { get; protected set; }
    
    public BT_CompositeNode(params BT_Node[] children)
    {
        Children = new List<BT_Node>(children);
    }

    public BT_Node Add(BT_Node node)
    {
        Children.Add(node);
        return node;
    }

    public override void AssignID(int parentId, ref int idCounter, Dictionary<int, BT_Node> nodes)
    {
        base.AssignID(parentId, ref idCounter, nodes);

        foreach (BT_Node child in Children)
        {
            child.AssignID(ID, ref idCounter, nodes);
        }
    }
}
