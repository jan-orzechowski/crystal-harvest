using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_CompositeNode : BT_Node
{
    public List<BT_Node> Children;

    public BT_CompositeNode() : base()
    {
        Children = new List<BT_Node>();
    }
    public BT_CompositeNode(params BT_Node[] children) : base()
    {
        Children = new List<BT_Node>(children);
    }

    public BT_Node Add(BT_Node node)
    {
        Children.Add(node);
        return node;
    }
}
