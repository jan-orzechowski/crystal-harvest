using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

public class Character : ISelectable 
{
    public string Name { get; protected set; }

    public Tile CurrentTile { get; protected set; }
    public Tile NextTile { get; protected set; }
    public Tile DestinationTile { get; protected set; }

    public float MovementPercentage { get; protected set; }
    float movementSpeed = 3f;

    AStar path;
    Pathfinder pathfinder;
    public bool PathNeedsReplacement;

    BT_Tree behaviourTree;
    BT_AgentMemory agentMemory;

    public int Resource { get; protected set; }

    SelectableDisplayObject DisplayObject;

    World world;

    public Character(string name, Tile currentTile, BT_Tree behaviourTree)
    {
        world = GameManager.Instance.World;

        Name = name;
        CurrentTile = currentTile;
        
        pathfinder = world.Pathfinder;
        MovementPercentage = 0f;

        this.behaviourTree = behaviourTree;
        agentMemory = new BT_AgentMemory(this);
    }
    
    public void UpdateCharacter(float deltaTime)
    {
        agentMemory.DeltaTime = deltaTime;
        behaviourTree.Tick(agentMemory);
        Move(deltaTime);
    }

    public void Move(float deltaTime)
    {
        if (DestinationTile == null || DestinationTile == CurrentTile)
        {
            DestinationTile = null;
            return;
        }

        if (path == null || path.Goal != DestinationTile )
        {
            TryGetNewPath();
            return;
        }

        if (PathNeedsReplacement)
        {
            TryGetNewPath();
        }
        
        if(path.GetLength() == 0 && NextTile != DestinationTile)
        {
            path = null;
            return;
        }

        if (NextTile == null)
        {
            NextTile = path.Dequeue();
        }

        // Czy w ogóle możemy wejść na to pole?
        if (NextTile.MovementCost == 0)
        {
            // Coś poszło nie tak
            NextTile = null;
            MovementPercentage = 0f;
            path = null;
            return;
        }

        if (MovementPercentage < 1.0f)
        {
            float meanMovementCost = (CurrentTile.MovementCost + NextTile.MovementCost) / 2;

            if (NextTile.X != CurrentTile.X || NextTile.Y != CurrentTile.Y)
            {
                MovementPercentage += ((deltaTime * movementSpeed) / meanMovementCost) / 1.414213562373f;
            }
            else
            {
                MovementPercentage += ((deltaTime * movementSpeed) / meanMovementCost); // / 1
            }
        }
        else
        {
            // Przechodzimy do nastepnego pola
            CurrentTile = NextTile;
            NextTile = null;
            MovementPercentage = 0f;
        }        
    }

    public void TryGetNewPath()
    {
        if (DestinationTile == null || DestinationTile == CurrentTile || DestinationTile.MovementCost == 0f)
        {
            DestinationTile = null;
            return;
        }

        AStar newPath = pathfinder.GetPath(this, CurrentTile, DestinationTile);

        if (newPath == null)
        {
            return;
        }

        if (newPath.IsReady)
        {
            if (newPath.IsImpossible)
            {
                DestinationTile = null;
                MovementPercentage = 0f;
                NextTile = null;
                path = null;
            }
            else
            {
                PathNeedsReplacement = false;
                path = newPath;

                // Czy nowa ścieżka pokrywa się ze starą co do następnego pola?
                if (path.Peek() == CurrentTile)
                {
                    path.Dequeue();
                }
                if (path.Peek() == NextTile)
                {
                    path.Dequeue();
                }
                else
                {
                    NextTile = null;
                    MovementPercentage = 0f;
                }
            }
        }
    }

    public void SetNewDestination(Tile tile)
    {
        if (DestinationTile == tile)
        {
            return;
        }
        else
        {
            DestinationTile = tile;
            NextTile = null;
            MovementPercentage = 0f;
            path = null;
        }        
    }

    public bool AddResource(int id)
    {
        if (Resource == 0)
        {
            Resource = id;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveResource()
    {
        Resource = 0;
    }

    public void AssignDisplayObject(SelectableDisplayObject displayObject)
    {
        DisplayObject = displayObject;
    }
    public string GetSelectionText()
    {
        string s = "";
        s += Name + "\n";
        if (DestinationTile != null)
        {
            s += "DestinationTile: " + DestinationTile.Position.ToString() + "\n";
        }
        else
        {
            s += "DestinationTile: brak \n";
        }
        if (NextTile != null)
        {
            s += "NextTile: " + NextTile.Position.ToString() + "\n";
        }
        else
        {
            s += "NextTile: brak \n";
        }
        s += "CurrentTile: " + CurrentTile.Position.ToString() + "\n";

        if ( Resource != 0)
        {
            s += "Resource: " + Resource + "\n";
        }

        return s;
    }
    public SelectableDisplayObject GetDisplayObject()
    {
        if (DisplayObject == null)
        {
            Debug.Log("Postać nie ma swojego modelu na mapie! " + Name + ", " + CurrentTile.Position.ToString());
            return null;
        } 
        else
        {
            return DisplayObject;
        }
    } 
}
