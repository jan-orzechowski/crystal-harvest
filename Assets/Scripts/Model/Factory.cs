using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Factory : IWorkplace, IBuildingModule
{
    public Building Building { get; protected set; }
    public BuildingPrototype Prototype { get { return Building.Prototype; } }

    public StorageWithRequirements InputStorage { get; protected set; }
    public Storage OutputStorage { get; protected set; }

    float productionTime;
    float productionTimeLeft;
    public bool ProductionStarted { get; protected set; }
  
    public Character WorkingCharacter { get; protected set; }
    Character jobReservation;
    float jobReservationTimer;
    float timeWithoutWork;

    public bool ProducesRobot { get; protected set; }

    public int RemainingProductionCycles { get; protected set; }
    public int StartingProductionCycles { get; protected set; }

    public bool IsNaturalDeposit { get; protected set; }

    public bool HidesCharacter { get { return Prototype.HidesCharacter; } }
    
    public bool Halted { get; protected set; }   

    bool preparingForDeconstruction;

    public Factory(Building building)
    {
        Building = building;
        productionTime = Prototype.ProductionTime;
        ProductionStarted = false;
        
        ProducesRobot = Prototype.ProducesRobot;
        IsNaturalDeposit = Prototype.IsNaturalDeposit;

        if (Prototype.ProductionCyclesLimitMax < 0)
        {
            RemainingProductionCycles = -1;
        }
        else
        {
            RemainingProductionCycles = UnityEngine.Random.Range(Prototype.ProductionCyclesLimitMin,
                                                                 Prototype.ProductionCyclesLimitMax + 1);
        }        
        StartingProductionCycles = RemainingProductionCycles;

        InputStorage = new StorageWithRequirements(Building, Prototype.ConsumedResources);
        OutputStorage = new Storage(Building, null);
    }

    public void UpdateFactory(float deltaTime)
    {                       
        if (WorkingCharacter == null)
        {
            jobReservationTimer -= deltaTime;
            if(jobReservationTimer < 0f)
            {
                jobReservation = null;
            }
        }

        if (timeWithoutWork > 0.2f)
        {
            WorkingCharacter = null;
        }

        timeWithoutWork += deltaTime;
    }

    public bool Work(float deltaTime, Character workingCharacter)
    {
        if (ProductionStarted == false 
            && (InputStorage.AreRequirementsMet == false || OutputStorage.IsEmpty == false))
        {
            return false;
        }

        if (WorkingCharacter != null && WorkingCharacter != workingCharacter)
        {
            return false;
        }

        jobReservation = null;

        if (ProductionStarted == false)
        {
            if (RemainingProductionCycles == 0)
            {
                return false;
            }

            if (Consume())
            {
                ProductionStarted = true;
                productionTimeLeft = productionTime;
                WorkingCharacter = workingCharacter;
                timeWithoutWork = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            productionTimeLeft -= deltaTime;
            WorkingCharacter = workingCharacter;
            timeWithoutWork = 0;

            if (productionTimeLeft <= 0)
            {
                if (Produce())
                {                 
                    ProductionStarted = false;
                    RemainingProductionCycles -= 1;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }        
    }

    bool Produce()
    {
        if (OutputStorage.IsEmpty)
        {
            OutputStorage = new Storage(Building, Prototype.ProducedResources, requiresEmptying: true);
            GameManager.Instance.World.RegisterResources(Prototype.ProducedResources);

            if (ProducesRobot)
            {
                Tile tile = Building.GetAccessTile(getSecond: true);
                if (tile == null) tile = Building.GetAccessTile(getSecond: false);
                return GameManager.Instance.World.CreateNewCharacter(tile.Position, true);
            }

            return true;
        }
        else
        {
            return false;
        }        
    }

    bool Consume()
    {
        if (InputStorage.AreRequirementsMet)
        {
            InputStorage = new StorageWithRequirements(Building, Prototype.ConsumedResources);
            GameManager.Instance.World.UnregisterResources(Prototype.ConsumedResources);
            return true;            
        }
        else
        {
            return false;
        }
    }

    public bool CanReserveJob(Character character)
    {
        return (WorkingCharacter == null
                && Halted == false
                && (jobReservation == null || jobReservation == character)
                && RemainingProductionCycles != 0
                && (ProductionStarted || (InputStorage.AreRequirementsMet && OutputStorage.IsEmpty)));
    }

    public bool ReserveJob(Character character)
    {
        if (CanReserveJob(character))
        {
            jobReservation = character;
            jobReservationTimer = 5f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RenewJobReservation(Character character)
    {
        if (jobReservation == character)
        {
            jobReservationTimer = 5f;
        }
    }

    public float GetCompletionPercentage()
    {
        if (ProductionStarted == false || productionTimeLeft <= 0)
        {
            return 0f;
        }
        else
        {
            return ((productionTime - productionTimeLeft) / productionTime);
        }
    }

    public void SetHalt(bool halted)
    {
        Halted = halted;
    }

    public void SetRemainingProductionCycles(int number)
    {
        RemainingProductionCycles = number;
        StartingProductionCycles = number;
    }

    public float GetRemainingProductionCyclesPercentage()
    {
        float result = 1f - ((float)(StartingProductionCycles - RemainingProductionCycles) / StartingProductionCycles);
        return result;
    }

    public void StartDeconstructionPreparation()
    {
        if (WorkingCharacter != null)
        {
            WorkingCharacter.InterruptActivity();
            WorkingCharacter = null;
        }

        ProductionStarted = false;
        Halted = true;
        jobReservation = null;
        InputStorage.StartDeconstructionPreparation();
        preparingForDeconstruction = true;
    }

    public void CancelDeconstructionPreparation()
    {
        Halted = false;
        InputStorage.CancelDeconstructionPreparation();
        preparingForDeconstruction = false;
    }

    public bool IsPreparingForDeconstruction()
    {
        return preparingForDeconstruction;
    }

    public bool IsReadyForDeconstruction()
    {
        return (InputStorage.IsReadyForDeconstruction() 
                && OutputStorage.IsReadyForDeconstruction()
                && Halted && WorkingCharacter == null);
    }

    public Tile GetAccessTile()
    {
        return Building.GetAccessTile();
    }

    public Rotation GetAccessTileRotation()
    {
        return Building.GetAccessTileRotation();
    }

    public string DEBUG_GetSelectionText()
    {
        string s = "";

        s += "Wstrzymane: " + Halted.ToString() + "\n";

        s += "Pracująca postać: ";
        if (WorkingCharacter != null)
        {
            s += WorkingCharacter.Name;
        }
        s += "\n";

        s += "Pozostały czas produkcji: ";
        if (ProductionStarted)
        {
            s += productionTimeLeft + "\n";
        }
        else
        {
            s += "nie rozpoczęta \n";
        }

        if(RemainingProductionCycles >= 0)
        {
            s += "Pozostałe cykle produkcji: " + RemainingProductionCycles + "\n";
        }

        s += InputStorage.DEBUG_GetSelectionText();
        s += OutputStorage.DEBUG_GetSelectionText();

        return s;
    }
}
