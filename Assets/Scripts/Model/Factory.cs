using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Factory : IWorkplace
{
    public Building Building { get; protected set; }
    BuildingPrototype prototype;

    public StorageToFill InputStorage { get; protected set; }
    public StorageToEmpty OutputStorage { get; protected set; }

    float productionTime;
    float productionTimeLeft;
    public bool ProductionStarted { get; protected set; }
  
    public Character WorkingCharacter { get; protected set; }
    bool jobReserved;
    float jobReservationTimer;
    float timeWithoutWork;

    public Factory(Building building, BuildingPrototype prototype)
    {
        Building = building;
        this.prototype = prototype;
        productionTime = prototype.ProductionTime;
        ProductionStarted = false;        

        InputStorage = new StorageToFill(Building, prototype.ConsumedResources);
        OutputStorage = new StorageToEmpty(Building, prototype.ProducedResources);
    }

    public void UpdateFactory(float deltaTime)
    {                       
        if (jobReserved)
        {
            jobReservationTimer -= deltaTime;
            if(jobReservationTimer < 0f)
            {
                jobReserved = false;
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
            && (InputStorage.IsFilled == false || OutputStorage.IsEmpty == false))
        {
            return false;
        }

        if (jobReserved)
        {
            jobReserved = false;
        }

        if (WorkingCharacter != null && WorkingCharacter != workingCharacter)
        {
            return false;
        }

        if (ProductionStarted == false)
        {
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
            OutputStorage = new StorageToEmpty(Building, prototype.ProducedResources);
            return true;
        }
        else
        {
            return false;
        }        
    }

    bool Consume()
    {
        if (InputStorage.IsFilled)
        {
            InputStorage = new StorageToFill(Building, prototype.ConsumedResources);
            return true;            
        }
        else
        {
            return false;
        }
    }

    public bool IsJobFree()
    {
        return (WorkingCharacter == null
                && jobReserved == false
                && (ProductionStarted || (InputStorage.IsFilled && OutputStorage.IsEmpty)));
    }

    public bool ReserveJob()
    {
        if (jobReserved)
        {
            return false;
        }
        else
        {
            jobReserved = true;
            jobReservationTimer = 10f;
            return true;
        }
    }
   
    public Tile GetAccessTile()
    {
        return Building.AccessTile;
    }

    public Rotation GetAccessTileRotation()
    {
        return Building.AccessTileRotation;
    }

    public string GetSelectionText()
    {
        string s = "";

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

        s += InputStorage.GetSelectionText();
        s += OutputStorage.GetSelectionText();

        return s;
    }
}
