using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum BT_Result : int
{
    FAILURE,
    SUCCESS,
    RUNNING,
    ERROR
}

public class BT_Tree
{
    int lastNodeID = 0;

    public BT_Node root;

    public BT_Result Tick(BT_AgentMemory am)
    {
        BT_Result result = root.Tick(am);
        return result;
    }

    void AssignIDs(BT_Node node)
    {
        // Funkcja wzywana rekursywnie dla każdego węzła

        if (node == null)
        {
            return;
        }

        node.ID = lastNodeID;
        lastNodeID++;

        if (node is BT_CompositeNode)
        {
            BT_CompositeNode compositeNode = (BT_CompositeNode)node;
            if (compositeNode.Children != null)
            {
                for (int i = 0; i < compositeNode.Children.Count; i++)
                {
                    AssignIDs(compositeNode.Children[i]);
                }
            }
        } 
        else if (node is BT_DecoratorNode)
        {
            AssignIDs((node as BT_DecoratorNode).Child);
        }
    }

    BT_CompositeNode Subtree(BT_CompositeNode parent, params BT_Node[] nodes)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            parent.Add(nodes[i]);
        }

        return parent;
    }

    BT_DecoratorNode Subtree(BT_DecoratorNode parent, BT_Node node)
    {
        parent.Add(node);
        return parent;
    }

    public void DEBUG_LoadTestTree()
    {
        root =
        Subtree(new BT_Priority(),
                    new BT_IsUsingService(),
                    new BT_Move(),
                    // Rezerwacje
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_HasResourceForReservation(),
                        new BT_GoToTargetStorage(),
                        new BT_Wait(1f),
                        new BT_DepositResource()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasReservation(),
                        new BT_GoToSourceStorage(),
                        new BT_Wait(1f),
                        new BT_TakeResource()
                        ),
                    // Potrzeby
                    Subtree(new BT_MemSequence(),
                        new BT_HasService(),
                        new BT_IsServiceReady(),
                        new BT_GoToService(),
                        new BT_StartUsingService()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_IsNeedHigherThan("Hunger", 0.5f),
                        new BT_FindService("Hunger")
                        ),
                    // Praca
                    Subtree(new BT_MemSequence(),
                        new BT_HasWorkplace(),
                        new BT_IsWorkplaceReadyForProduction(),
                        new BT_ReserveJob(),
                        new BT_GoToWorkplace(),
                        new BT_Wait(1f),
                        new BT_Work()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_HasWorkplace(),
                        new BT_Inverter(new BT_IsWorkplaceReadyForProduction()),
                        new BT_GetTransportJobForWorkplace()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_GetRandomWorkplace()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_GetTransportJob()
                        ),
                    Subtree(new BT_MemSequence(),
                        new BT_GoToRandomTile(4),
                        new BT_WaitRandom(1f, 2f)
                        )
        );

        AssignIDs(root);
    }
}
