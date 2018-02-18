using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Storage : ISourceStorage, ITargetStorage
{
    public Building Building { get; protected set; }

    public Dictionary<int, int> Resources { get; protected set; }
    public Dictionary<Character, int> ReservedResources { get; protected set; }
    public List<Character> FreeSpaceReservations { get; protected set; }
    public int MaxCapacity { get; protected set; }
    public int CurrentResourceCount { get; protected set; }
    public int UnreservedFreeSpace { get { return (MaxCapacity - CurrentResourceCount - FreeSpaceReservations.Count); } }

    public Storage(Building building, BuildingPrototype prototype)
    {
        Building = building;
       
        ReservedResources = new Dictionary<Character, int>();
        FreeSpaceReservations = new List<Character>();

        MaxCapacity = prototype.MaxStorage;
        
        if(prototype.InitialStorage != null)
        {
            int initialStorageCount = 0;
            foreach (int id in prototype.InitialStorage.Keys)
            {
                initialStorageCount += prototype.InitialStorage[id];
            }
            if (initialStorageCount <= MaxCapacity)
            {
                Resources = new Dictionary<int, int>(prototype.InitialStorage);
                CurrentResourceCount = initialStorageCount;
                return;
            }           
        }

        Resources = new Dictionary<int, int>();
        CurrentResourceCount = 0;
    }
    
    public bool CanReserveFreeSpace(Character character)
    {
        return (UnreservedFreeSpace > 0
                && FreeSpaceReservations.Contains(character) == false);
    }
    public bool ReserveFreeSpace(int resourceID, Character character)
    {
        if (CanReserveFreeSpace(character))
        {
            FreeSpaceReservations.Add(character);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveFreeSpaceReservation(Character character)
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
    public bool CanReserveResource(int resourceID, Character character)
    {
        return (character.Reservation == null
                && ReservedResources.ContainsKey(character) == false
                && Resources.ContainsKey(resourceID)
                && Resources[resourceID] > 0);        
    }
    public bool ReserveResource(int resourceID, Character character)
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

    public bool RemoveResourceReservation(Character character)
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

    public bool TransferFromStorage(int resourceID, Character character)
    {
        if (ReservedResources.ContainsKey(character) &&
            ReservedResources[character] == resourceID
            && character.HasResource == false)
        {
            ReservedResources.Remove(character);
            character.AddResource(resourceID);
            CurrentResourceCount--;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TransferToStorage(int resourceID, Character character)
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
            return true;
        }
        return false;
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
        return Building.AccessTile;
    }

    public Rotation GetAccessTileRotation()
    {
        return Building.AccessTileRotation;
    }

    public string GetSelectionText()
    {
        string s = "";
        s += "Zasoby: " + "\n";
        var resourcesToPrint = GetAllResources();
        foreach (int resourceID in resourcesToPrint.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + resourcesToPrint[resourceID] + "\n";
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
            s += "- " + character.Name + "- " + GameManager.Instance.World.ResourcesInfo[ReservedResources[character]].Name + "\n";
        }

        return s;
    }
}
