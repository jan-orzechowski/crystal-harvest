using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StorageToFill : ITargetStorage
{
    public Dictionary<int, int> Resources { get; protected set; }
    public Dictionary<Character, int> PendingResources { get; protected set; }
    public Dictionary<int, int> MissingResources { get; protected set; }
    public int MissingResourcesCount { get; protected set; }
    public bool IsFilled { get { return (MissingResourcesCount == 0); } }

    Tile accessTile;
    Rotation accessTileRotation;

    public bool Halted;

    public StorageToFill(Tile accessTile, Rotation accessTileRotation, Dictionary<int, int> requiredResources)
    {
        this.accessTile = accessTile;
        this.accessTileRotation = accessTileRotation;

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

        Halted = false;
    }

    public StorageToFill(Building building, Dictionary<int, int> requiredResources)
        : this(building.GetAccessTile(), building.GetAccessTileRotation(), requiredResources) { }

    public bool CanReserveFreeSpace(int resourceID, Character character)
    {
        return (Halted == false
                && character.Reservation == null
                && MissingResources.ContainsKey(resourceID)
                && PendingResources.ContainsKey(character) == false);
    }

    public bool TransferToStorage(int resourceID, Character character)
    {
        if (PendingResources.ContainsKey(character)
            && PendingResources[character] == resourceID
            && character.Resource == resourceID)
        {
            PendingResources.Remove(character);
            MissingResourcesCount--;
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
            return true;
        }
        return false;
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
            PendingResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveFreeSpaceReservation(Character character)
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

    public bool IsWaitingForResources()
    {
        return (PendingResources.Keys.Count > 0);
    }

    public Tile GetAccessTile()
    {
        return accessTile;
    }

    public Rotation GetAccessTileRotation()
    {
        return accessTileRotation;
    }

    public string GetSelectionText()
    {
        string s = "";

        s += "StorageToFill - jest: " + "\n";
        foreach (int resourceID in Resources.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - "
                + Resources[resourceID] + "\n";
        }
        s += "\n";
        s += "StorageToFill - brakuje: " + "\n";
        if (MissingResourcesCount > 0)
            foreach (int resourceID in MissingResources.Keys)
            {
                s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - "
                    + MissingResources[resourceID] + "\n";
            }

        s += "\n";
        s += "StorageToFill - w drodze: " + "\n";
        foreach (Character character in PendingResources.Keys)
        {
            s += "- " + character.Name + " - "
                + GameManager.Instance.World.ResourcesInfo[PendingResources[character]].Name + "\n";
        }
        s += "\n";

        return s;
    }
}   
