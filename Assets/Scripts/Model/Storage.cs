using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Storage : IBuildingModule
{
    public Building Building { get; protected set; }
    
    public Dictionary<int, int> Resources { get; protected set; }
    public Dictionary<Character, int> ReservedResources { get; protected set; }
    public List<Character> FreeSpaceReservations { get; protected set; }
    public int MaxCapacity { get; protected set; }
    public int CurrentResourceCount { get; protected set; }

    public bool RequiresEmptying { get; protected set; }
    
    public bool Changed = true;
    
    public bool IsEmpty { get { return (CurrentResourceCount == 0); } }
    public int UnreservedFreeSpace { get { return (MaxCapacity - CurrentResourceCount - FreeSpaceReservations.Count); } }
    public bool HidesCharacter { get { return Prototype.HidesCharacter; } }
    public BuildingPrototype Prototype { get { return Building.Prototype; } }

    bool preparingForDeconstruction;

    public bool Halted { get { return preparingForDeconstruction; } }

    public Storage (Building building, Dictionary<int, int> initialResources, bool requiresEmptying = false)
    {       
        Building = building;
               
        if (initialResources == null)
        {
            Resources = new Dictionary<int, int>();
            CurrentResourceCount = 0;
        }
        else
        {
            Resources = new Dictionary<int, int>(initialResources);
            foreach (int id in Resources.Keys)
            {
                CurrentResourceCount += Resources[id];
            }
            GameManager.Instance.World.RegisterResources(initialResources);
        }
       
        ReservedResources = new Dictionary<Character, int>();
        FreeSpaceReservations = new List<Character>();

        if (CurrentResourceCount > MaxCapacity)
        {
            MaxCapacity = CurrentResourceCount;
        }
        else
        {
            MaxCapacity = Prototype.MaxStorage;
        }
    }
    
    public virtual bool CanReserveFreeSpace(int resourceID, Character character)
    {
        return (RequiresEmptying == false
                && UnreservedFreeSpace > 0
                && FreeSpaceReservations.Contains(character) == false
                && Building.Prototype.RestrictedResources.Contains(resourceID) == false);
    }

    public virtual bool ReserveFreeSpace(int resourceID, Character character)
    {
        if (CanReserveFreeSpace(resourceID, character))
        {
            FreeSpaceReservations.Add(character);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool RemoveFreeSpaceReservation(Character character)
    {
        if (FreeSpaceReservations.Contains(character))
        {
            FreeSpaceReservations.Remove(character);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CanReserveResource(int resourceID, Character character)
    {
        return (character.Reservation == null
                && ReservedResources.ContainsKey(character) == false
                && Resources.ContainsKey(resourceID)
                && Resources[resourceID] > 0);        
    }

    public virtual bool ReserveResource(int resourceID, Character character)
    {
        if (CanReserveResource(resourceID, character))
        {
            Resources[resourceID] = Resources[resourceID] - 1;
            if (Resources[resourceID] == 0)
            {
                Resources.Remove(resourceID);
            }

            ReservedResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool RemoveResourceReservation(Character character)
    {
        if (ReservedResources.ContainsKey(character))
        {
            int resourceID = ReservedResources[character];            

            if (Resources.ContainsKey(resourceID))
            {
                Resources[resourceID] = Resources[resourceID] + 1;
            }
            else
            {
                Resources[resourceID] = 1;
            }

            ReservedResources.Remove(character);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool TransferFromStorage(int resourceID, Character character)
    {
        if (ReservedResources.ContainsKey(character) &&
            ReservedResources[character] == resourceID
            && character.HasResource == false)
        {
            ReservedResources.Remove(character);
            character.AddResource(resourceID);
            CurrentResourceCount--;
            Changed = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool TransferToStorage(int resourceID, Character character)
    {
        if (FreeSpaceReservations.Contains(character) &&
            character.Resource == resourceID)
        {
            FreeSpaceReservations.Remove(character);
            if (Resources.ContainsKey(resourceID))
            {
                Resources[resourceID] = Resources[resourceID] + 1;
            }
            else
            {
                Resources[resourceID] = 1;
            }
            CurrentResourceCount++;
            character.RemoveResource();
            character.ReservationUsed();
            Changed = true;
            return true;
        }
        return false;
    }

    public void StartDeconstructionPreparation()
    {
        RequiresEmptying = true;
        preparingForDeconstruction = true;
    }

    public void CancelDeconstructionPreparation()
    {
        RequiresEmptying = false;
        preparingForDeconstruction = false;
    }

    public bool IsPreparingForDeconstruction()
    {
        return preparingForDeconstruction;
    }

    public virtual bool IsReadyForDeconstruction()
    {
        return (CurrentResourceCount == 0 && FreeSpaceReservations.Count == 0);
    }

    public void SetHalt(bool halt)
    {
        return;
    }

    public Dictionary<int, int> GetAllResources()
    {
        Dictionary<int, int> result = new Dictionary<int, int>(Resources);
        foreach (int resourceID in ReservedResources.Values)
        {
            if (result.ContainsKey(resourceID))
            {
                result[resourceID] = result[resourceID] + 1;
            }
            else
            {
                result[resourceID] = 1;
            }
        }
        return result;
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

        s += "Tryb opróżniania: " + RequiresEmptying + "\n";

        s += "Zasoby: " + "\n";
        var resourcesToPrint = GetAllResources();
        foreach (int resourceID in resourcesToPrint.Keys)
        {
            s += "- " + GameManager.Instance.GetResourceName(resourceID) + " - " + resourcesToPrint[resourceID] + "\n";
        }
        s += "\n";

        s += "Wolne miejsca: " + (UnreservedFreeSpace + FreeSpaceReservations.Count) + "\n";

        s += "Rezerwacje wolnych miejsc: " + "\n";
        foreach (Character character in FreeSpaceReservations)
        {
            s += "- " + character.Name + "\n";
        }
        s += "\n";

        s += "Rezerwacje zasobów: " + "\n";
        foreach (Character character in ReservedResources.Keys)
        {
            s += "- " + character.Name + "- " + GameManager.Instance.GetResourceName(ReservedResources[character]) + "\n";
        }

        return s;
    }
}
