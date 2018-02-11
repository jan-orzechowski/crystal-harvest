using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : ISelectable 
{
    public string Type;
    public List<Tile> Tiles { get; protected set; }
    public Rotation Rotation { get; protected set; }

    public Tile AccessTile { get; protected set; }
    public Rotation AccessTileRotation { get; protected set; }

    public SelectableDisplayObject DisplayObject;

    public bool DoesNotBlockAccess { get; protected set; }

    public IStorage IStorage { get; protected set; }

    public Building(BuildingPrototype prototype, Rotation buildingRotation, List<Tile> tiles, Tile accessTile)
    {
        Type = prototype.Type;
        Rotation = buildingRotation;
        Tiles = tiles;

        if (prototype.HasAccessTile)
        {
            AccessTile = accessTile;
            AccessTileRotation = prototype.NormalizedAccessTileRotation.Rotate(buildingRotation);
        }

        DoesNotBlockAccess = prototype.DoesNotBlockAccess;
        
        if (prototype.ProductionTime > 0f)
        {
            IStorage = new Factory(this, prototype);
        }     
        else if (prototype.MaxStorage > 0)
        {
            IStorage = new Storage(this, prototype);
        }                
    }
    public void UpdateBuilding(float deltaTime)
    {
        
    }    

    public void AssignDisplayObject(SelectableDisplayObject displayObject)
    {
        DisplayObject = displayObject;
    }
    public string GetSelectionText()
    {
        string s = "";
        s += Type + "\n";
        s += "Obrót: " + Rotation + ". Obrót pola dostępu: " + AccessTileRotation + "\n";
        if (IStorage != null)
        {
            s += IStorage.GetSelectionText();
        }
        return s;
    }
    public SelectableDisplayObject GetDisplayObject()
    {
        if (DisplayObject == null)
        {
            Debug.Log("Budynek nie posiada modelu na mapie: " + Tiles[0].Position.ToString());
            return null;
        } 
        else
        {
            return DisplayObject;
        }
    } 
}
