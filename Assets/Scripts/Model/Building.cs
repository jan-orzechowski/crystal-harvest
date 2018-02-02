using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : ISelectable 
{
    public string Type;
    public List<Tile> Tiles;
    public Rotation Rotation;

    public SelectableDisplayObject DisplayObject;    

    public Building(string type, Rotation rotation, List<Tile> tiles)
    {
        Type = type;
        Rotation = rotation;
        Tiles = tiles;
       
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
