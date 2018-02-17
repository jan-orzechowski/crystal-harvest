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

    public Factory Factory;
    public Service Service;
    public Storage Storage;

    BuildingPrototype prototype;

    public Building(BuildingPrototype buildingPrototype, Rotation buildingRotation, List<Tile> tiles, Tile accessTile)
    {
        prototype = buildingPrototype;

        Type = prototype.Type;
        Rotation = buildingRotation;
        Tiles = tiles;

        if (prototype.HasAccessTile)
        {
            AccessTile = accessTile;
            AccessTileRotation = prototype.NormalizedAccessTileRotation.Rotate(buildingRotation);
        }

        DoesNotBlockAccess = prototype.DoesNotBlockAccess;
    }

    public void LoadDataForFinishedBuilding()
    {
        if (prototype.ProductionTime > 0f)
        {
            Factory = new Factory(this, prototype);
        }
        else if (prototype.NeedFulfilled != null)
        {
            Service = new Service(this, prototype);
        }
        else if (prototype.MaxStorage > 0)
        {
            Storage = new Storage(this, prototype);
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
        if (Factory != null)
        {
            s += Factory.GetSelectionText();
        }
        if (Storage != null)
        {
            s += Storage.GetSelectionText();
        }
        if (Service != null)
        {
            s += Service.GetSelectionText();
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
