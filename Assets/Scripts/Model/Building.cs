using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Building : ISelectable 
{
    public string Type { get { return Prototype.Type; } }
    public List<Tile> Tiles { get; protected set; }
    public Rotation Rotation { get; protected set; }

    Tile accessTile;
    Rotation accessTileRotation;
    Tile secondAccessTile;
    Rotation secondAccessTileRotation;

    public SelectableDisplayObject DisplayObject;

    public IBuildingModule Module;

    public BuildingPrototype Prototype { get; protected set; }

    public Building(BuildingPrototype buildingPrototype, Rotation buildingRotation, 
                    List<Tile> tiles, Tile accessTile, Tile secondAccessTile)
    {
        Prototype = buildingPrototype;

        Rotation = buildingRotation;
        Tiles = tiles;

        if (Prototype.HasAccessTile)
        {
            this.accessTile = accessTile;
            accessTileRotation = Prototype.NormalizedAccessTileRotation.Rotate(buildingRotation);
        }

        if (Prototype.HasSecondAccessTile && secondAccessTile != null)
        {
            this.secondAccessTile = secondAccessTile;
            secondAccessTileRotation = Prototype.NormalizedSecondAccessTileRotation.Rotate(buildingRotation);
        }
    }

    public void LoadDataForFinishedBuilding()
    {
        if (Prototype.ProductionTime > 0f)
        {
            Module = new Factory(this);
        }
        else if (Prototype.NeedFulfilled != null)
        {
            Module = new Service(this);
        }
        else if (Prototype.MaxStorage > 0)
        {
            Module = new Storage(this, Prototype.InitialStorage, false);
        }        
    }

    public void AssignDisplayObject(SelectableDisplayObject displayObject)
    {
        DisplayObject = displayObject;
    }

    public Tile GetAccessTile(bool getSecond = false)
    {
        if (getSecond)
        {
            return secondAccessTile;
        }
        else
        {
            if (accessTile == null || Tile.CheckPassability(accessTile) == false)
            {
                GetNewUnreservedAccessTile();
            }

            return accessTile;
        }                
    }

    public Rotation GetAccessTileRotation(bool getSecond = false)
    {
        if (getSecond)
        {
            return secondAccessTileRotation;
        }
        else
        {
            if (accessTile == null || Tile.CheckPassability(accessTile) == false)
            {
                GetNewUnreservedAccessTile();
            }

            return accessTileRotation;
        }
    }

    Tile GetNewUnreservedAccessTile()
    {
        foreach (Tile tileToCheck in Tiles)
        {
            Tile[] neighbours = tileToCheck.GetNeighbours();
            foreach (Tile neighbour in neighbours)
            {
                if (Tile.CheckPassability(neighbour))
                {
                    accessTile = neighbour;
                    accessTileRotation = RotationMethods.GetRotationTowardsPosition(
                        positionToBeRotated: neighbour.Position,
                        positionToRotateAt: tileToCheck.Position);
                    return accessTile;
                }
            }

            if (Prototype.CanBeAccessedFromTop == false) break;

            neighbours = tileToCheck.GetUpperNeighbours();
            foreach (Tile neighbour in neighbours)
            {
                if (Tile.CheckPassability(neighbour))
                {
                    accessTile = neighbour;
                    accessTileRotation = RotationMethods.GetRotationTowardsPosition(
                        positionToBeRotated: neighbour.Position,
                        positionToRotateAt: tileToCheck.Position);
                    return accessTile;
                }
            }
        }

        accessTile = null;
        accessTileRotation = Rotation.N;
        return null;
    }
   
    public string DEBUG_GetSelectionText()
    {
        string s = "";
        s += Type + "\n";
        s += "Obrót: " + Rotation + ". Obrót pola dostępu: " + accessTileRotation + "\n";
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
