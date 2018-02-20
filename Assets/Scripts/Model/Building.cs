using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : ISelectable 
{
    public string Type { get { return prototype.Type; } }
    public List<Tile> Tiles { get; protected set; }
    public Rotation Rotation { get; protected set; }

    public Tile AccessTile { get; protected set; }
    public Rotation AccessTileRotation { get; protected set; }
    public Tile SecondAccessTile { get; protected set; }
    public Rotation SecondAccessTileRotation { get; protected set; }


    public SelectableDisplayObject DisplayObject;

    public IBuildingModule Module;

    BuildingPrototype prototype;

    public Building(BuildingPrototype buildingPrototype, Rotation buildingRotation, 
                    List<Tile> tiles, Tile accessTile, Tile secondAccessTile)
    {
        prototype = buildingPrototype;

        Rotation = buildingRotation;
        Tiles = tiles;

        if (prototype.HasAccessTile)
        {
            AccessTile = accessTile;
            AccessTileRotation = prototype.NormalizedAccessTileRotation.Rotate(buildingRotation);
        }

        if (prototype.HasSecondAccessTile && secondAccessTile != null)
        {
            SecondAccessTile = secondAccessTile;
            SecondAccessTileRotation = prototype.NormalizedSecondAccessTileRotation.Rotate(buildingRotation);
        }
    }

    public void LoadDataForFinishedBuilding()
    {
        if (prototype.ProductionTime > 0f)
        {
            Module = new Factory(this, prototype);
        }
        else if (prototype.NeedFulfilled != null)
        {
            Module = new Service(this, prototype);
        }
        else if (prototype.MaxStorage > 0)
        {
            Module = new Storage(this, prototype);
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
        if (Module != null)
        {
            s += Module.GetSelectionText();
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
