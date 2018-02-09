using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_DecoratorNode : BT_Node
{
    public BT_Node Child;

    public BT_DecoratorNode(BT_Node child)
    {
        Child = child;
    }

    public BT_Node Add(BT_Node node)
    {
        Child = node;
        return node;
    }
}
