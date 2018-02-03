using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BuildMode
{
    Single,
    Multiple,
    Corridor
}

public class BuildModeManager : MonoBehaviour 
{
    public BuildMode BuildMode { get; protected set; }
  
    BuildingPrototype currentPrototype;
   
    Rotation currentRotation;

    World world;

    void Start () 
    {
        world = GameManager.Instance.World;
    }
    
    public bool Build(TilePosition start, TilePosition end)
    {        
        if (currentPrototype != null)
        {
            if(BuildMode == BuildMode.Multiple)
            {
                // To zakłada, że budynki budowane po wiele sztuk zajmują tylko po jednym polu każdy
                for (int x = Math.Min(start.X, end.X);
                    x <= Math.Max(start.X, end.X);
                    x++)
                {
                    for (int y = Math.Min(start.Y, end.Y);
                        y <= Math.Max(start.Y, end.Y);
                        y++)
                    {
                        world.PlaceNewBuilding(
                            new TilePosition(x,y,start.Height),
                            currentRotation,
                            currentPrototype);
                    }
                }
            }
            else if (BuildMode == BuildMode.Single)
            {
                world.PlaceNewBuilding(start, currentRotation, currentPrototype);
            }            
        }
        return true;
    }

    public void ShowBuildPreview(TilePosition start, TilePosition end)
    {        
        if (start == end)
        {
            GameManager.Instance.ShowPreview(
                            start,
                            currentRotation,
                            currentPrototype);
        }
        else if (BuildMode == BuildMode.Multiple)
        {
            for (int x = Math.Min(start.X, end.X);
                x <= Math.Max(start.X, end.X);
                x++)
            {
                for (int y = Math.Min(start.Y, end.Y);
                    y <= Math.Max(start.Y, end.Y);
                    y++)
                {
                    if (world.IsValidBuildingPosition(
                            new TilePosition(x, y, start.Height),
                            currentRotation,
                            currentPrototype)
                        )
                    {
                        GameManager.Instance.ShowPreview(
                            new TilePosition(x, y, start.Height),
                            currentRotation,
                            currentPrototype);
                    }
                }
            }
        }        
    }

    public void SetBuildingType(string type)
    {
        currentPrototype = world.GetBuildingPrototype(type);
        currentRotation = Rotation.N;

        if (type == "Debug1")
        {
            BuildMode = BuildMode.Multiple;
        }
        else if (type == "Debug2")
        {
            BuildMode = BuildMode.Single;
        }
    }

    public void Rotate()
    {
        if (currentPrototype.AllowRotation)
        {
            if      (currentRotation == Rotation.N) currentRotation = Rotation.E;
            else if (currentRotation == Rotation.E) currentRotation = Rotation.S;
            else if (currentRotation == Rotation.S) currentRotation = Rotation.W;
            else     currentRotation = Rotation.N;
        }
        else
        {
            Debug.Log("Tego budynku nie można obracać");
        }
    }
}
