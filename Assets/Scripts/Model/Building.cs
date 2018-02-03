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

    public bool DoesNotBlockAccess;

    public Building(BuildingPrototype prototype, Rotation rotation, List<Tile> tiles, Tile accessTile)
    {
        Type = prototype.Type;
        Rotation = rotation;
        Tiles = tiles;
        AccessTile = accessTile;
        DoesNotBlockAccess = prototype.DoesNotBlockAccess;
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
        return Type;
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
