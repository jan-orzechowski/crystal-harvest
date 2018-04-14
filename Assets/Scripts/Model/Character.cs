using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Pathfinding;

public class Character : ISelectable 
{
    public string Name { get; protected set; }
    public Tile CurrentTile { get; protected set; }
    public Tile NextTile { get; protected set; }
    public Tile DestinationTile { get; protected set; }

    public float MovementPercentage { get; protected set; }
    float movementSpeed = 9f; // 3f

    public Quaternion CurrentRotation { get; protected set; }
    Quaternion targetRotation;
    float degreesPerSecond = 450f; // 270

    bool isLastTileRotationSet;
    Quaternion lastTileRotation;

    AStar path;
    Pathfinder pathfinder;
    public bool PathNeedsReplacement;

    BT_Tree behaviourTree;
    BT_AgentMemory agentMemory;

    public int Resource { get; protected set; }
    public bool HasResource { get { return (Resource != 0); } }

    public ResourceReservation Reservation { get; protected set; }

    public Dictionary<string, float> Needs;
    public Dictionary<string, float> NeedGrowthPerSecond;

    public CharacterDisplayObject DisplayObject { get; protected set; }

    World world;

    public bool UsingService;

    public bool IsRobot { get; protected set; }

    public Character(string name, Tile currentTile, BT_Tree behaviourTree, bool isRobot)
    {
        this.Name = name;
        world = GameManager.Instance.World;
        
        CurrentTile = currentTile;
        IsRobot = isRobot;
        
        pathfinder = world.Pathfinder;
        MovementPercentage = 0f;

        this.behaviourTree = behaviourTree;
        agentMemory = new BT_AgentMemory(this);

        StaticData.LoadNeeds(this);

        CurrentRotation = Quaternion.identity;
        targetRotation = Quaternion.identity;
        lastTileRotation = Quaternion.identity;
        isLastTileRotationSet = false;
    }
    
    public void UpdateCharacter(float deltaTime)
    {
        agentMemory.DeltaTime = deltaTime;
        behaviourTree.Tick(agentMemory);
        UpdateNeeds(deltaTime);
        if (UsingService == false)
        {
            Move(deltaTime);
        }
    }

    public void Move(float deltaTime)
    {
        if (DestinationTile == CurrentTile) { DestinationTile = null; }

        if (DestinationTile == null)
        {
            if (isLastTileRotationSet && AreRotationsEqualInYAxis(CurrentRotation, lastTileRotation) == false)
            {
                CurrentRotation = Quaternion.RotateTowards(
                                    CurrentRotation, lastTileRotation,
                                    deltaTime * degreesPerSecond);

                if (AreRotationsEqualInYAxis(CurrentRotation, lastTileRotation))
                {
                    isLastTileRotationSet = false;
                }
            }
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
        
        // Tutaj występuje NullReferenceException gdy budynek jest niedostępny
        if (path.GetLength() == 0 && NextTile != DestinationTile)
        {
            path = null;
            return;
        }

        if (NextTile == null)
        {
            NextTile = path.Dequeue();
            targetRotation = GetRotationForNextTile(NextTile);
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

        if(AreRotationsEqualInYAxis(CurrentRotation, targetRotation) == false)
        {
            CurrentRotation = Quaternion.RotateTowards(
                CurrentRotation, targetRotation,
                deltaTime * degreesPerSecond);
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
                //Debug.Log("Ścieżka z " 
                //    + newPath.Start.Position.ToString() + " do " 
                //    + newPath.Goal.Position.ToString() + " niemożliwa.");

                DestinationTile = null;
                isLastTileRotationSet = false;
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
            //Debug.Log("Nowy cel: " + tile.Position.ToString());
            DestinationTile = tile;
            NextTile = null;
            MovementPercentage = 0f;
            path = null;
        }        
    }

    public void SetNewDestination(Tile tile, Rotation finalRotation)
    {
        SetNewDestination(tile);
        SetLastTileRotation(finalRotation);
    }

    public void SetLastTileRotation(Rotation rotation)
    {
        isLastTileRotationSet = true;
        lastTileRotation = Quaternion.Euler(rotation.ToEulerAngles());
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

    public bool SetNewReservation(ResourceReservation reservation)
    {
        if (Reservation != null)
        {
            return false;
        }
        else
        {
            Reservation = reservation;
            return true;
        }
    }

    public void ReservationUsed()
    {
        Reservation = null;
    }

    public void ServiceEnded()
    {
        UsingService = false;
        agentMemory.SetNewService(null);
    }

    void UpdateNeeds(float deltaTime)
    {
        foreach (string need in Needs.Keys.ToList())
        {
            if (NeedGrowthPerSecond.ContainsKey(need))
                Needs[need] = Needs[need] + (NeedGrowthPerSecond[need] * deltaTime);
            if (Needs[need] > 1f)
                Needs[need] = 1f;
        }
    }

    Quaternion GetRotationForNextTile(Tile nextTile)
    {
        Vector3 rotationVector = new Vector3(nextTile.X - CurrentTile.X, 0, nextTile.Y - CurrentTile.Y);
        float rotationAngle = Vector3.SignedAngle(Vector3.back, rotationVector, Vector3.up);
        return Quaternion.Euler(new Vector3(0f, rotationAngle, 0f));
    }

    bool AreRotationsEqualInYAxis(Quaternion quaternionA, Quaternion quaternionB)
    {
        float range = 0.01f;
        return (Mathf.Abs(quaternionA.eulerAngles.y - quaternionB.eulerAngles.y) <= range);
    }

    public bool IsMoving()
    {
        return (MovementPercentage > 0.2f
                || (NextTile != null && AreRotationsEqualInYAxis(CurrentRotation, targetRotation) == false)
                || (isLastTileRotationSet && CurrentTile == DestinationTile
                        && AreRotationsEqualInYAxis(CurrentRotation, lastTileRotation) == false)
                );
    }

    public void InterruptActivity()
    {
        agentMemory.SetNewWorkplace(null);
        agentMemory.SetNewService(null);
        UsingService = false;
    }

    public void AssignDisplayObject(SelectableDisplayObject displayObject)
    {
        if (displayObject is CharacterDisplayObject)
        {
            DisplayObject = (CharacterDisplayObject)displayObject;
        }
        else
        {
            Debug.LogWarning("Niewłaściwy SelectableDisplayObject dla postaci");
        }
        
    }

    public string DEBUG_GetSelectionText()
    {
        string s = "";
        s += Name + "\n";

        foreach(string need in Needs.Keys)
        {
            s += need + ": " + Needs[need] + "\n";
        }
        
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

        s += "lastTileRotation: " + lastTileRotation.eulerAngles.ToString() + "\n";

        s += "CurrentRotation: " + CurrentRotation.eulerAngles.ToString() + "\n";
        s += "TargetRotation: " + targetRotation.eulerAngles.ToString() + "\n";

        if (HasResource)
        {
            s += "Resource: " + GameManager.Instance.World.ResourcesInfo[Resource].Name + "\n";
        }

        return s;
    }

    public SelectableDisplayObject GetDisplayObject()
    {
        if (DisplayObject == null)
        {
            Debug.Log("Postać nie ma swojego modelu na mapie! Pozycja: " + CurrentTile.Position.ToString());
            return null;
        } 
        else
        {
            return DisplayObject;
        }
    } 
}