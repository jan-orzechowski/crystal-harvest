using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BuildMode
{
    Single,
    Multiple,
    SingleInstant
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
                        world.PlaceNewConstructionSite(
                            new TilePosition(x, y, start.Height),
                            currentRotation,
                            currentPrototype);
                    }
                }
            }
            else if (BuildMode == BuildMode.Single)
            {
                world.PlaceNewConstructionSite(GetPositionForBuilding(start), currentRotation, currentPrototype);
            }
            else if (BuildMode == BuildMode.SingleInstant)
            {
                world.InstantBuild(GetPositionForBuilding(start), currentRotation, currentPrototype);
            }
        }
        return true;
    }

    public void ShowBuildPreview(TilePosition start, TilePosition end)
    {        
        if (start == end)
        {
            GameManager.Instance.ShowPreview(
                            GetPositionForBuilding(start),
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

    TilePosition GetPositionForBuilding(TilePosition mousePosition)
    {
        return (mousePosition - world.MapNormalizedPositionToWorld(currentPrototype.MousePivotPoint, 
                                                                    new TilePosition(0, 0, 0),
                                                                    currentRotation));
    }

    public void SetBuildingType(string type)
    {
        currentPrototype = world.GetBuildingPrototype(type);
        currentRotation = currentPrototype.StartingRotation;

        if (type == "Debug1")
        {
            // BuildMode = BuildMode.Multiple;
        }
        else if (type == "Debug2")
        {
            BuildMode = BuildMode.Single;
        }
        else if (type == "Stairs" || type == "OreDeposit")
        {
            BuildMode = BuildMode.SingleInstant;
        }
    }

    public void ToggleInstantBuildMode()
    {
        if (BuildMode == BuildMode.Single)
        {
            Debug.Log("BuildMode - Single Instant");
            BuildMode = BuildMode.SingleInstant;
        }
        else if (BuildMode == BuildMode.SingleInstant)
        {
            Debug.Log("BuildMode - Single");
            BuildMode = BuildMode.Single;
        }        
    }

    public void Rotate()
    {
        if (currentPrototype.AllowRotation)
        {
            currentRotation = currentRotation.Rotate(Rotation.E);
        }
        else
        {
            Debug.Log("Tego budynku nie można obracać");
        }
    }
}
