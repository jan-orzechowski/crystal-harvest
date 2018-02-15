using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Factory : IWorkplace
{
    public Building Building { get; protected set; }

    public Dictionary<int, int> InputResources { get; protected set; }
    public Dictionary<Character, int> PendingInputResources { get; protected set; }
    public Dictionary<int, int> MissingResources { get; protected set; }
    public int MissingResourcesCount { get; protected set; }

    public Dictionary<int, int> OutputResources { get; protected set; }
    public int ResourcesToRemoveCount { get; protected set; }
    public Dictionary<Character, int> ReservedOutputResources { get; protected set; }

    float productionTime;
    float productionTimeLeft;
    public bool ProductionStarted { get; protected set; }

    public Dictionary<int, int> ResourcesConsumedAtStart { get; protected set; }
    int resourcesConsumedAtStartCount;
    public Dictionary<int, int> ResourcesProducedAtTheEnd { get; protected set; }
    int resourcesProducedAtTheEndCount;

    public Character WorkingCharacter { get; protected set; }
    bool jobReserved;
    float jobReservationTimer;
    float timeWithoutWork;

    public Factory(Building building, BuildingPrototype prototype)
    {
        Building = building;
        productionTime = prototype.ProductionTime;
        ProductionStarted = false;

        InputResources = new Dictionary<int, int>();
        PendingInputResources = new Dictionary<Character, int>();        
        OutputResources = new Dictionary<int, int>();
        ReservedOutputResources = new Dictionary<Character, int>();

        ResourcesConsumedAtStart = prototype.ConsumedResources;        
        resourcesConsumedAtStartCount = 0;

        if (ResourcesConsumedAtStart != null)
        {
            foreach (int id in ResourcesConsumedAtStart.Keys)
            {
                resourcesConsumedAtStartCount += ResourcesConsumedAtStart[id];
            }
            MissingResources = new Dictionary<int, int>(ResourcesConsumedAtStart);
            MissingResourcesCount = resourcesConsumedAtStartCount;
        }
        else
        {
            MissingResourcesCount = 0;
        }
                
        ResourcesProducedAtTheEnd = prototype.ProducedResources;
        resourcesProducedAtTheEndCount = 0;
        if (ResourcesProducedAtTheEnd != null)
        {            
            foreach (int id in ResourcesProducedAtTheEnd.Keys)
            {
                resourcesProducedAtTheEndCount += ResourcesProducedAtTheEnd[id];
            }
        }
        else
        {
            Debug.Log("Fabryka musi coś produkować!");
        }
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
        if (ProductionStarted == false && (MissingResourcesCount > 0 || ResourcesToRemoveCount > 0))
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
        if (ResourcesToRemoveCount == 0)
        {
            OutputResources = new Dictionary<int, int>(ResourcesProducedAtTheEnd);
            ResourcesToRemoveCount = resourcesProducedAtTheEndCount;
            return true;
        }
        else
        {
            return false;
        }        
    }

    bool Consume()
    {
        if (MissingResourcesCount > 0) { return false; }
        if (ResourcesConsumedAtStart == null) { return true; }

        foreach (int resourceID in ResourcesConsumedAtStart.Keys)
        {
            if (InputResources.ContainsKey(resourceID) && InputResources[resourceID] == ResourcesConsumedAtStart[resourceID])
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        InputResources.Clear();
        MissingResources = new Dictionary<int, int>(ResourcesConsumedAtStart);
        MissingResourcesCount = resourcesConsumedAtStartCount;
        return true;
    }

    public bool IsJobFree()
    {
        return (WorkingCharacter == null
                && jobReserved == false
                && (ProductionStarted || (MissingResourcesCount == 0 && ResourcesToRemoveCount == 0)));
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

    public bool CanReserveFreeSpace(int resourceID, Character character)
    {
        return (MissingResources.ContainsKey(resourceID)
                && PendingInputResources.ContainsKey(character) == false);
    }
    public bool ReserveFreeSpace(int resourceID, Character character)
    {
        if (CanReserveFreeSpace(resourceID, character))
        {
            MissingResources[resourceID] = MissingResources[resourceID] - 1;            
            if (MissingResources[resourceID] == 0)
            {
                MissingResources.Remove(resourceID);
            }
            PendingInputResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveFreeSpaceReservation(Character character)
    {
        if (PendingInputResources.ContainsKey(character))
        {
            int resourceID = PendingInputResources[character];
            PendingInputResources.Remove(character);
            
            if (MissingResources.ContainsKey(resourceID))
            {
                MissingResources[resourceID] = MissingResources[resourceID] - 1;
            }
            else
            {
                MissingResources[resourceID] = 1;
            }
            MissingResourcesCount++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReserveResource(int resourceID, Character character)
    {
        return (ReservedOutputResources.ContainsKey(character) == false 
                && OutputResources.ContainsKey(resourceID) && OutputResources[resourceID] > 0);
    }

    public bool ReserveResource(int resourceID, Character character)
    {
        if (CanReserveResource(resourceID, character))
        {
            OutputResources[resourceID] = OutputResources[resourceID] - 1;
            if (OutputResources[resourceID] == 0)
            {
                OutputResources.Remove(resourceID);
            }

            ReservedOutputResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveResourceReservation(Character character)
    {
        if (ReservedOutputResources.ContainsKey(character))
        {
            int resourceID = ReservedOutputResources[character];

            if (OutputResources.ContainsKey(resourceID))
            {
                OutputResources[resourceID] = OutputResources[resourceID] + 1;
            }
            else
            {
                OutputResources[resourceID] = 1;
            }

            ReservedOutputResources.Remove(character);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TransferFromStorage(int resourceID, Character character)
    {
        if (ReservedOutputResources.ContainsKey(character)
            && ReservedOutputResources[character] == resourceID)
        {
            ReservedOutputResources.Remove(character);
            ResourcesToRemoveCount--;
            character.AddResource(resourceID);
            return true;
        }
        return false;
    }

    public bool TransferToStorage(int resourceID, Character character)
    {
        if (PendingInputResources.ContainsKey(character)
            && PendingInputResources[character] == resourceID
            && character.Resource == resourceID)
        {
            PendingInputResources.Remove(character);
            MissingResourcesCount--;
            if (InputResources.ContainsKey(resourceID))
            {
                InputResources[resourceID] = InputResources[resourceID] + 1;
            }
            else
            {
                InputResources[resourceID] = 1;
            }
            character.RemoveResource();
            return true;
        }
        return false;
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
                      
        s += "Input - jest: " + "\n";
        foreach (int resourceID in InputResources.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + InputResources[resourceID] + "\n";
        }
        s += "\n";
        s += "Input - brakuje: " + "\n";
        if (MissingResourcesCount > 0)
            foreach (int resourceID in MissingResources.Keys)
            {
                s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + MissingResources[resourceID] + "\n";
            }

        s += "\n";
        s += "Input - w drodze: " + "\n";
        foreach (Character character in PendingInputResources.Keys)
        {
            s += "- " + character.Name + " - " + GameManager.Instance.World.ResourcesInfo[PendingInputResources[character]].Name + "\n";
        }
        s += "\n";
        s += "Output: " + "\n";
        foreach (int resourceID in OutputResources.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + OutputResources[resourceID] + "\n";
        }
        s += "\n";
        s += "Rezerwacje zasobów w output: " + "\n";
        foreach (Character character in ReservedOutputResources.Keys)
        {
            s += "- " + character.Name + "- " + GameManager.Instance.World.ResourcesInfo[ReservedOutputResources[character]].Name + "\n";
        }

        return s;
    }
}
