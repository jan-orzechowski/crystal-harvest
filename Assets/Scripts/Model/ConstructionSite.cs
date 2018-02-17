using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ConstructionStage
{
    ScaffoldingConstruction,
    Construction,
    Deconstruction,
    ScaffoldingDeconstruction
}

public class ConstructionSite : IWorkplace
{
    public Building Building;

    StorageToFill ConstructionStorage;
    StorageToEmpty DeconstructionStorage;

    public StorageToFill InputStorage { get { return ConstructionStorage;  } }
    public StorageToEmpty OutputStorage { get { return DeconstructionStorage; } }

    float constructionTime;
    float deconstructionTime;

    float stageTimeLeft;
    static float scaffoldingStageTime = 5f;

    public ConstructionStage Stage { get; protected set; }

    public bool ConstructionMode   { get { return (Stage == ConstructionStage.Construction 
                                            || Stage == ConstructionStage.ScaffoldingConstruction);  } }
    public bool DeconstructionMode { get { return (Stage == ConstructionStage.Deconstruction 
                                            || Stage == ConstructionStage.ScaffoldingDeconstruction);  } }

    public Character WorkingCharacter { get; protected set; }
    bool jobReserved;
    float jobReservationTimer;
    float timeWithoutWork;

    BuildingPrototype prototype;
    World world;

    public ConstructionSite(Building building, BuildingPrototype buildingPrototype)
    {
        prototype = buildingPrototype;
        world = GameManager.Instance.World;

        Building = building;
        constructionTime = prototype.ConstructionTime;
        deconstructionTime = constructionTime / 2;

        Stage = ConstructionStage.ScaffoldingConstruction;
        LoadResourcesForScaffoldingConstruction();
        stageTimeLeft = scaffoldingStageTime;
    }

    public void UpdateConstructionSite(float deltaTime)
    {
        if (jobReserved)
        {
            jobReservationTimer -= deltaTime;
            if (jobReservationTimer < 0f)
            {
                jobReserved = false;
            }
        }

        if (timeWithoutWork > 0.2f)
        {
            WorkingCharacter = null;
        }

        timeWithoutWork += deltaTime;
    }

    public bool Work(float deltaTime, Character workingCharacter)
    {
        if (ConstructionMode && ConstructionStorage.IsFilled == false)
        {
            return false;
        }
        else if (DeconstructionMode && DeconstructionStorage.IsEmpty == false)
        {
            return false;
        }

        if (jobReserved)
        {
            jobReserved = false;
        }

        if (WorkingCharacter != null && WorkingCharacter != workingCharacter)
        {
            return false;
        }

        stageTimeLeft -= deltaTime;
        WorkingCharacter = workingCharacter;
        timeWithoutWork = 0;

        if (stageTimeLeft <= 0)
        {
            WorkingCharacter = null;
            if (Stage == ConstructionStage.ScaffoldingConstruction)
            {
                Stage = ConstructionStage.Construction;                
                stageTimeLeft = constructionTime;
                LoadResourcesForConstruction();
            }
            else if (Stage == ConstructionStage.Construction)
            {                
                world.FinishConstruction(this);
            }
            else if (Stage == ConstructionStage.Deconstruction)
            {
                Stage = ConstructionStage.ScaffoldingDeconstruction;
                stageTimeLeft = scaffoldingStageTime;
            }
            else if (Stage == ConstructionStage.ScaffoldingDeconstruction)
            {
                world.FinishDeconstruction(this);
            }
        }

        return true;        
    }
 
    public bool IsJobFree()
    {
        return (WorkingCharacter == null
                && jobReserved == false
                && ((ConstructionMode && ConstructionStorage.IsFilled)
                    || DeconstructionMode && DeconstructionStorage.IsEmpty));
    }

    public bool ReserveJob()
    {
        if (jobReserved)
        {
            return false;
        }
        else
        {
            jobReserved = true;
            jobReservationTimer = 10f;
            return true;
        }
    }

    void LoadResourcesForConstruction()
    {      
        ConstructionStorage = new StorageToFill(Building, prototype.ConstructionResources);
        DeconstructionStorage = new StorageToEmpty(Building, null);
    }

    void LoadResourcesFromDeconstruction()
    {
        ConstructionStorage = new StorageToFill(Building, null);
        DeconstructionStorage = new StorageToEmpty(Building, prototype.ResourcesFromDeconstruction);
    }

    void LoadResourcesForScaffoldingConstruction()
    {
        ConstructionStorage = new StorageToFill(Building, new Dictionary<int, int>() { { 1, 1 } });
        DeconstructionStorage = new StorageToEmpty(Building, null);
    }

    public float GetCompletionPercentage()
    {
        float result;
        if (ConstructionMode)
        {
            float totalTime = constructionTime + scaffoldingStageTime;
            if (Stage == ConstructionStage.ScaffoldingConstruction)
            {
                result = (totalTime - stageTimeLeft - constructionTime) / totalTime;
            }
            else
            {
                result = (totalTime - stageTimeLeft) / totalTime;            
            }
        }
        else
        {
            float totalTime = deconstructionTime + scaffoldingStageTime;
            // W tym wypadku liczymy na odwrót - rusztowanie jest rozbierane na końcu
            if (Stage == ConstructionStage.ScaffoldingDeconstruction)
            {
                result = (totalTime - stageTimeLeft) / totalTime;
            }
            else
            {
                result = (totalTime - stageTimeLeft - scaffoldingStageTime) / totalTime;
            }
        }
        return result;
    }

    public float GetStageCompletionPercentage()
    {
        float result = 0;
        if(Stage == ConstructionStage.ScaffoldingConstruction 
            || Stage == ConstructionStage.ScaffoldingDeconstruction)        
        {
            result = (scaffoldingStageTime - stageTimeLeft) / scaffoldingStageTime;
        }
        if (Stage == ConstructionStage.Construction)
        {
            result = (constructionTime - stageTimeLeft) / constructionTime;
        }
        if (Stage == ConstructionStage.Deconstruction)
        {
            result = (deconstructionTime - stageTimeLeft) / deconstructionTime;
        }
        return result;
    }

    public Tile GetAccessTile()
    {
        if (Building != null)
            return Building.AccessTile;
        else return null;
    }

    public Rotation GetAccessTileRotation()
    {
        if (Building != null)
            return Building.AccessTileRotation;
        else return Rotation.N;
    }
  
    public string GetSelectionText()
    {
        string s = "";

        s += "Pracująca postać: ";
        if (WorkingCharacter != null)
        {
            s += WorkingCharacter.Name;
        }
        s += "\n";

        s += "Pozostały czas konstrukcji: ";
        if (ConstructionMode)
        {
            s += stageTimeLeft + "\n";
        }
        else
        {
            s += "nie rozpoczęta \n";
        }

        if (ConstructionMode)
        {
            s += ConstructionStorage.GetSelectionText();
        }
        else if (DeconstructionMode)
        {
            s += DeconstructionStorage.GetSelectionText();
        }

        return s;
    }
}
