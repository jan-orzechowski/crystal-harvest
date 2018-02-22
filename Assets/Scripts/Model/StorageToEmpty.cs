using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StorageToEmpty : ISourceStorage
{
    public Dictionary<int, int> ResourcesToRemove { get; protected set; }
    public Dictionary<Character, int> ReservedResources { get; protected set; }
    public int ResourcesToRemoveCount { get; protected set; }
    public bool IsEmpty { get { return (ResourcesToRemoveCount == 0); } }

    Tile accessTile;
    Rotation accessTileRotation;

    public StorageToEmpty(Tile accessTile, Rotation accessTileRotation, Dictionary<int, int> resourcesToRemove)
    {
        this.accessTile = accessTile;
        this.accessTileRotation = accessTileRotation;

        ReservedResources = new Dictionary<Character, int>();

        if (resourcesToRemove == null)
        {
            ResourcesToRemove = new Dictionary<int, int>();
            ResourcesToRemoveCount = 0;
            return;
        }
        else
        {
            ResourcesToRemove = new Dictionary<int, int>(resourcesToRemove);
            foreach (int id in ResourcesToRemove.Keys)
            {
                ResourcesToRemoveCount += ResourcesToRemove[id];
            }
        }
    }

    public StorageToEmpty(Building building, Dictionary<int, int> resourcesToRemove)
        : this(building.GetAccessTile(), building.GetAccessTileRotation(), resourcesToRemove) { }

    public virtual bool CanReserveResource(int resourceID, Character character)
    {
        return (character.Reservation == null
                && ReservedResources.ContainsKey(character) == false
                && ResourcesToRemove.ContainsKey(resourceID) && ResourcesToRemove[resourceID] > 0);
    }

    public bool ReserveResource(int resourceID, Character character)
    {
        if (CanReserveResource(resourceID, character))
        {
            ResourcesToRemove[resourceID] = ResourcesToRemove[resourceID] - 1;
            if (ResourcesToRemove[resourceID] == 0)
            {
                ResourcesToRemove.Remove(resourceID);
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

            if (ResourcesToRemove.ContainsKey(resourceID))
            {
                ResourcesToRemove[resourceID] = ResourcesToRemove[resourceID] + 1;
            }
            else
            {
                ResourcesToRemove[resourceID] = 1;
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
        if (ReservedResources.ContainsKey(character)
            && ReservedResources[character] == resourceID)
        {
            ReservedResources.Remove(character);
            ResourcesToRemoveCount--;
            character.AddResource(resourceID);
            return true;
        }
        return false;
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

        s += "StorageToEmpty - jest: " + "\n";
        foreach (int resourceID in ResourcesToRemove.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - "
                + ResourcesToRemove[resourceID] + "\n";
        }
        s += "\n";
        s += "StorageToEmpty - rezerwacje: " + "\n";
        foreach (Character character in ReservedResources.Keys)
        {
            s += "- " + character.Name + "- "
                + GameManager.Instance.World.ResourcesInfo[ReservedResources[character]].Name + "\n";
        }

        return s;
    }
}
