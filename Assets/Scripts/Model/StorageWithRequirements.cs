using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StorageWithRequirements : Storage
{
    public Dictionary<int, int> MissingResources { get; protected set; }
    public Dictionary<Character, int> PendingResources { get; protected set; }
    public int MissingResourcesCount { get; protected set; }
    public bool AreRequirementsMet { get { return (MissingResourcesCount == 0); } }
        
    public StorageWithRequirements(Building building, Dictionary<int, int> requiredResources)
        : base (building, null, false)
    {
        Resources = new Dictionary<int, int>();
        PendingResources = new Dictionary<Character, int>();

        if (requiredResources == null)
        {
            MissingResources = new Dictionary<int, int>();
            MissingResourcesCount = 0;
            return;
        }
        else
        {
            MissingResources = new Dictionary<int, int>(requiredResources);
            foreach (int id in requiredResources.Keys)
            {
                MissingResourcesCount += requiredResources[id];
            }
        }
    }

    public override bool CanReserveFreeSpace(int resourceID, Character character)
    {
        return (RequiresEmptying == false
                && character.Reservation == null
                && MissingResources.ContainsKey(resourceID)
                && PendingResources.ContainsKey(character) == false);
    }

    public override bool TransferToStorage(int resourceID, Character character)
    {
        if (PendingResources.ContainsKey(character)
            && PendingResources[character] == resourceID
            && character.Resource == resourceID)
        {
            PendingResources.Remove(character);
            MissingResourcesCount--;
            CurrentResourceCount++;

            if (Resources.ContainsKey(resourceID))
            {
                Resources[resourceID] = Resources[resourceID] + 1;
            }
            else
            {
                Resources[resourceID] = 1;
            }
            character.RemoveResource();
            character.ReservationUsed();
            Changed = true;
            return true;
        }
        return false;
    }

    public override bool ReserveFreeSpace(int resourceID, Character character)
    {
        if (CanReserveFreeSpace(resourceID, character))
        {
            MissingResources[resourceID] = MissingResources[resourceID] - 1;
            if (MissingResources[resourceID] == 0)
            {
                MissingResources.Remove(resourceID);
            }
            PendingResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool RemoveFreeSpaceReservation(Character character)
    {
        if (PendingResources.ContainsKey(character))
        {
            int resourceID = PendingResources[character];
            PendingResources.Remove(character);

            if (MissingResources.ContainsKey(resourceID))
            {
                MissingResources[resourceID] = MissingResources[resourceID] - 1;
            }
            else
            {
                MissingResources[resourceID] = 1;
            }            
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool CanReserveResource(int resourceID, Character character)
    {
        return (RequiresEmptying == true
                && character.Reservation == null
                && ReservedResources.ContainsKey(character) == false
                && Resources.ContainsKey(resourceID)
                && Resources[resourceID] > 0);
    }
    
    public override bool TransferFromStorage(int resourceID, Character character)
    {
        if (ReservedResources.ContainsKey(character) &&
            ReservedResources[character] == resourceID
            && character.HasResource == false)
        {
            ReservedResources.Remove(character);
            character.AddResource(resourceID);

            if (MissingResources.ContainsKey(resourceID))
            {
                MissingResources[resourceID] = MissingResources[resourceID] + 1;
            }
            else
            {
                MissingResources.Add(resourceID, 1);
            }            
            MissingResourcesCount++;

            CurrentResourceCount--;
            Changed = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWaitingForResources()
    {
        return (PendingResources.Keys.Count > 0);
    }

    public override bool IsReadyForDeconstruction()
    {
        return (base.IsReadyForDeconstruction() && IsWaitingForResources() == false);
    }
 
    public new string DEBUG_GetSelectionText()
    {
        string s = "";

        s += "Tryb opróżniania: " + RequiresEmptying + "\n";

        s += "StorageWithRequirements - jest: " + "\n";
        foreach (int resourceID in Resources.Keys)
        {
            s += "- " + GameManager.Instance.GetResourceName(resourceID) + " - "
                + Resources[resourceID] + "\n";
        }
        s += "\n";
        s += "StorageWithRequirements - brakuje: " + "\n";
        if (MissingResourcesCount > 0)
            foreach (int resourceID in MissingResources.Keys)
            {
                s += "- " + GameManager.Instance.GetResourceName(resourceID) + " - "
                    + MissingResources[resourceID] + "\n";
            }

        s += "\n";
        s += "StorageWithRequirements - w drodze: " + "\n";
        foreach (Character character in PendingResources.Keys)
        {
            s += "- " + character.Name + " - "
                + GameManager.Instance.GetResourceName(PendingResources[character]) + "\n";
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
