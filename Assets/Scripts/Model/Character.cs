using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Pathfinding;

public enum CharacterState
{
    PreparingForDeletion,
    PreparingForCheckingPath,
    CheckingPath,
    Movement,
    IdleWithPath,
    IdleAtDestination,
    UsingService
}


public class Character : ISelectable
{
    public string Name { get; protected set; }
    public Tile CurrentTile { get; protected set; }
    public Tile NextTile { get; protected set; }
    public Tile DestinationTile { get; protected set; }

    public CharacterState State { get; protected set; }

    public float MovementPercentage { get; protected set; }
    float movementSpeed = 9f; // 3f

    public Quaternion CurrentRotation { get; protected set; }
    Quaternion targetRotation;
    float degreesPerSecond = 450f; // 270

    bool isLastTileRotationSet;
    Quaternion lastTileRotation;

    public AStar Path { get; protected set; }
    static Pathfinder pathfinder;
    public bool PathNeedsReplacement;

    BT_Tree behaviourTree;
    BT_AgentMemory agentMemory;

    public int Resource { get; protected set; }
    public bool HasResource { get { return (Resource != 0); } }

    public ResourceReservation Reservation { get; protected set; }

    public Dictionary<string, float> Needs;
    public Dictionary<string, float> NeedGrowthPerSecond;

    public CharacterDisplayObject DisplayObject { get; protected set; }

    static World world;

    public bool IsRobot { get; protected set; }

    static float inaccessibleTileDefaultTimer = 20f;
    Dictionary<Tile, float> inaccessibleTilesTimers;

    public Character(string name, Tile currentTile, BT_Tree behaviourTree, bool isRobot)
    {
        Name = name;
        world = GameManager.Instance.World;

        State = CharacterState.IdleAtDestination;

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

        inaccessibleTilesTimers = new Dictionary<Tile, float>();

        DestinationTile = CurrentTile;
    }

    public void UpdateCharacter(float deltaTime)
    {
        agentMemory.DeltaTime = deltaTime;
        behaviourTree.Tick(agentMemory);
        UpdateNeeds(deltaTime);

        if (State == CharacterState.CheckingPath)
        {
            TryGetNewPath();
        }

        if (State == CharacterState.Movement
            || State == CharacterState.PreparingForCheckingPath
            || State == CharacterState.PreparingForDeletion)
        {
            Move(deltaTime);
        }
        
        if (State == CharacterState.PreparingForDeletion)
        {
            DisplayObject.PlayDeathAnimation();
        }

        foreach (Tile tile in inaccessibleTilesTimers.Keys.ToList())
        {
            inaccessibleTilesTimers[tile] -= deltaTime;
            if (inaccessibleTilesTimers[tile] <= 0)
            {
                inaccessibleTilesTimers.Remove(tile);
            }
        }
    }

    #region Movement

    public void Move(float deltaTime)
    {
        if (DestinationTile == null
            || IsTileMarkedAsInaccessible(DestinationTile))
        {
            DestinationTile = CurrentTile;
        }

        // Obrót na ostatnim polu
        if (CurrentTile == DestinationTile)
        {
            if (isLastTileRotationSet
                && AreRotationsEqualInYAxis(CurrentRotation, lastTileRotation) == false)
            {
                CurrentRotation = Quaternion.RotateTowards(
                                     CurrentRotation, lastTileRotation,
                                     deltaTime * degreesPerSecond);

                if (AreRotationsEqualInYAxis(CurrentRotation, lastTileRotation))
                {
                    isLastTileRotationSet = false;
                    if (State == CharacterState.PreparingForDeletion) return;        
                    else State = CharacterState.IdleAtDestination;
                }
            }
            else
            {
                if (State == CharacterState.PreparingForDeletion) return;
                else State = CharacterState.IdleAtDestination;
            }
            return;
        }
        
        if (Path == null || Path.Goal != DestinationTile)
        {
            TryGetNewPath();
            return;
        }

        if (PathNeedsReplacement)
        {
            TryGetNewPath();

            if (Path == null) return;
        }
        
        if (NextTile == null)
        {
            // Nie bierzemy następnego pola - zatrzymujemy się
            if (State == CharacterState.PreparingForCheckingPath)
            {
                State = CharacterState.CheckingPath;
            }
            else if (State == CharacterState.PreparingForDeletion)
            {
                return;
            }
            
            NextTile = Path.Dequeue();
            if (NextTile == null)
            {
                Path = null;
                return;
            }         
            targetRotation = GetRotationForNextTile(NextTile);              
        }

        if (NextTile.MovementCost == 0)
        {
            // Coś poszło nie tak
            NextTile = null;
            MovementPercentage = 0f;
            Path = null;
            return;
        }

        // Obrót w kierunku następnego pola
        if (AreRotationsEqualInYAxis(CurrentRotation, targetRotation) == false)
        {
            CurrentRotation = Quaternion.RotateTowards(
                CurrentRotation, targetRotation,
                deltaTime * degreesPerSecond);
            return;
        }

        // Ruch po linii prostej
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
            // Przechodzimy do następnego pola
            CurrentTile = NextTile;            
            NextTile = null;
            MovementPercentage = 0f;
        }        
    }

    void TryGetNewPath()
    {
        if (DestinationTile == CurrentTile
            || State == CharacterState.PreparingForDeletion)
        {
            return;
        }

        if (DestinationTile.MovementCost == 0f)
        {
            MarkTileAsInaccessible(DestinationTile);
        }

        if (IsTileMarkedAsInaccessible(DestinationTile))
        {
            CurrentTile = DestinationTile;
            State = CharacterState.IdleAtDestination;
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

                MarkTileAsInaccessible(newPath.Goal);
                DestinationTile = CurrentTile;

                isLastTileRotationSet = false;
                MovementPercentage = 0f;
                NextTile = null;
                Path = null;

                State = CharacterState.IdleAtDestination;                
            }
            else
            {
                PathNeedsReplacement = false;
                Path = newPath;

                if (State == CharacterState.CheckingPath) State = CharacterState.IdleWithPath;

                // Czy nowa ścieżka pokrywa się ze starą co do następnego pola?
                if (Path.Peek() == CurrentTile)
                {
                    Path.Dequeue();
                }
                if (Path.Peek() == NextTile)
                {
                    Path.Dequeue();
                }
                else
                {
                    NextTile = null;
                    MovementPercentage = 0f;
                }
            }
        }
    }
    
    public bool SetNewDestination(Tile tile, bool startMovement)
    {
        if (State == CharacterState.PreparingForDeletion
            || State == CharacterState.UsingService)
        {
            return false;
        }

        if (tile == null || inaccessibleTilesTimers.ContainsKey(tile))
        {
            DestinationTile = CurrentTile;
            return false;
        }

        if (tile == DestinationTile)
        {
            if (startMovement) State = CharacterState.Movement;
            return true;
        }

        DestinationTile = tile;

        if (Path != null && Path.Goal != tile)
        {
            NextTile = null;
            MovementPercentage = 0f;
            Path = null;
        }
     
        State = startMovement ? CharacterState.Movement : CharacterState.PreparingForCheckingPath;

        return true;                
    }
        
    public bool IsTileMarkedAsInaccessible(Tile tile)
    {
        if (tile == null) return true;
        else
        {
            // Debug.Log(tile.ToString() + " inaccessible: " + inaccessibleTilesTimers.ContainsKey(tile));
            return inaccessibleTilesTimers.ContainsKey(tile);
        }
    }

    public bool AreBothAccessTilesMarkedAsInaccessbile(IAccessible building)
    {
        return (IsTileMarkedAsInaccessible(building.GetAccessTile(false))
                && IsTileMarkedAsInaccessible(building.GetAccessTile(true)));
    }

    void MarkTileAsInaccessible(Tile tile)
    {
        if (inaccessibleTilesTimers.ContainsKey(tile))
        {
            inaccessibleTilesTimers[tile] = inaccessibleTileDefaultTimer;
        }
        else
        {
            inaccessibleTilesTimers.Add(tile, inaccessibleTileDefaultTimer);
        }
        // Debug.Log(tile.ToString() + " - oznaczone jako niedostępne");
    }

    public void SetLastTileRotation(Rotation rotation)
    {
        isLastTileRotationSet = true;
        lastTileRotation = Quaternion.Euler(rotation.ToEulerAngles());
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
    
    #endregion

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
        State = CharacterState.IdleAtDestination;
        agentMemory.Service = null;
        agentMemory.UseServiceSecondAccessTile = false;
    }
    
    public void WorkFinished()
    {
        agentMemory.Workplace = null;
        agentMemory.UseWorkplaceSecondAccessTile = false;
    }

    public void InterruptActivity()
    {
        agentMemory.Workplace = null;
        agentMemory.UseWorkplaceSecondAccessTile = false;
        agentMemory.Service = null;
        agentMemory.UseServiceSecondAccessTile = false;

        if (State == CharacterState.UsingService) State = CharacterState.IdleAtDestination;
    }

    public void StartUsingService()
    {
        State = CharacterState.UsingService;
    }
 
    public void StartPreparingForDeletion()
    {
        State = CharacterState.PreparingForDeletion;
        InterruptActivity();
    }

    public bool IsReadyForDeletion()
    {
        if (State != CharacterState.PreparingForDeletion) return false;

        return (agentMemory.Workplace == null 
                && agentMemory.Service == null
                && NextTile == null 
                && DisplayObject.DeathAnimationPlayed
                && MovementPercentage <= 0.05f);
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

    public void AssignDisplayObject(SelectableDisplayObject displayObject)
    {
        if (displayObject is CharacterDisplayObject)
        {
            DisplayObject = (CharacterDisplayObject)displayObject;
        }
        else if (displayObject == null)
        {
            DisplayObject = null;
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

        s += "State: " + State.ToString() + "\n";

        s += "Reservation: " + ((Reservation != null) ? Reservation.ToString() : "") + "\n";

        s += "Workplace: " + ((agentMemory.Workplace != null && agentMemory.Workplace.Building != null) 
                                ? agentMemory.Workplace.Building.Name : "") + "\n";

        foreach (string need in Needs.Keys)
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
            s += "Resource: " + GameManager.Instance.GetResourceName(Resource) + "\n";
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