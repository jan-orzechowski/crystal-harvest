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
    public Dictionary<int, int> Resources { get; protected set; }
    public Dictionary<Character, int> PendingResources { get; protected set; }
    public Dictionary<Character, int> ReservedResources { get; protected set; }
    public Dictionary<int, int> MissingResources { get; protected set; }
    public int MissingResourcesCount { get; protected set; }
    public int ResourcesToRemoveCount { get; protected set; }

    public Dictionary<int, int> OutputResources { get { if (DeconstructionMode) { return Resources; } else { return null; } } }

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
        
        Resources = new Dictionary<int, int>();
        PendingResources = new Dictionary<Character, int>();      
        ReservedResources = new Dictionary<Character, int>();
      
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
        if (ConstructionMode && MissingResourcesCount > 0)
        {
            return false;
        }
        else if (DeconstructionMode && ResourcesToRemoveCount > 0)
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
                Resources.Clear();
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
                ResourcesToRemoveCount = 0;
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
                && ((ConstructionMode && MissingResourcesCount == 0)
                    || DeconstructionMode && ResourcesToRemoveCount == 0));
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

    public bool CanReserveFreeSpace(int resourceID, Character character)
    {
        return ((Stage == ConstructionStage.Deconstruction || Stage == ConstructionStage.ScaffoldingDeconstruction) == false 
                && MissingResources.ContainsKey(resourceID)
                && PendingResources.ContainsKey(character) == false);
    }
    public bool ReserveFreeSpace(int resourceID, Character character)
    {
        if (Stage == ConstructionStage.Deconstruction || Stage == ConstructionStage.ScaffoldingDeconstruction)
        {
            return false;
        }

        if (CanReserveFreeSpace(resourceID, character))
        {
            MissingResources[resourceID] = MissingResources[resourceID] - 1;
            if (MissingResources[resourceID] == 0)
            {
                MissingResources.Remove(resourceID);
            }
            PendingResources.Add(character, resourceID);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveFreeSpaceReservation(Character character)
    {
        if (PendingResources.ContainsKey(character))
        {
            int resourceID = PendingResources[character];
            PendingResources.Remove(character);

            if (MissingResources.ContainsKey(resourceID))
            {
                MissingResources[resourceID] = MissingResources[resourceID] - 1;
            }
            else
            {
                MissingResources[resourceID] = 1;
            }
            MissingResourcesCount++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReserveResource(int resourceID, Character character)
    {
        return (DeconstructionMode
                && ReservedResources.ContainsKey(character) == false
                && Resources.ContainsKey(resourceID) && Resources[resourceID] > 0);
    }

    public bool ReserveResource(int resourceID, Character character)
    {
        if (DeconstructionMode == false)
        {
            return false;
        }

        if (CanReserveResource(resourceID, character))
        {
            Resources[resourceID] = Resources[resourceID] - 1;
            if (Resources[resourceID] == 0)
            {
                Resources.Remove(resourceID);
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

            if (Resources.ContainsKey(resourceID))
            {
                Resources[resourceID] = Resources[resourceID] + 1;
            }
            else
            {
                Resources[resourceID] = 1;
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
        if(DeconstructionMode == false)
        {
            return false;
        }

        if (ReservedResources.ContainsKey(character)
            && ReservedResources[character] == resourceID)
        {
            ReservedResources.Remove(character);
            character.AddResource(resourceID);
            return true;
        }
        return false;
    }

    public bool TransferToStorage(int resourceID, Character character)
    {
        if (DeconstructionMode)
        {
            return false;
        }

        if (PendingResources.ContainsKey(character)
            && PendingResources[character] == resourceID
            && character.Resource == resourceID)
        {
            PendingResources.Remove(character);
            MissingResourcesCount--;
            if (Resources.ContainsKey(resourceID))
            {
                Resources[resourceID] = Resources[resourceID] + 1;
            }
            else
            {
                Resources[resourceID] = 1;
            }
            character.RemoveResource();
            return true;
        }
        return false;
    }

    void LoadResourcesForConstruction()
    {      
        MissingResources = new Dictionary<int, int>(prototype.ConstructionResources);
        MissingResourcesCount = 0;

        if (MissingResources != null)
        {
            foreach (int id in MissingResources.Keys)
            {
                MissingResourcesCount += MissingResources[id];
            }
        }
    }

    void LoadResourcesFromDeconstruction()
    {
        Resources = new Dictionary<int, int>(prototype.ResourcesFromDeconstruction);
        ResourcesToRemoveCount = 0;
        if (Resources != null)
        {
            foreach (int id in Resources.Keys)
            {
                ResourcesToRemoveCount += Resources[id];
            }
        }
    }

    void LoadResourcesForScaffoldingConstruction()
    {
        MissingResources = new Dictionary<int, int>() { { 1, 1 } };
        MissingResourcesCount = 1;
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

    public int GetMissingResourcesCount()
    {
        return MissingResourcesCount;
    }

    public int GetOutputResourcesCount()
    {
        return ResourcesToRemoveCount;
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

        //s += "Pozostały czas konstrukcji: ";
        //if (ConstructionMode)
        //{
        //    s += stageTimeLeft + "\n";
        //}
        //else
        //{
        //    s += "nie rozpoczęta \n";
        //}

        s += "Potrzebne zasoby - są: " + "\n";
        foreach (int resourceID in Resources.Keys)
        {
            s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + Resources[resourceID] + "\n";
        }
        s += "\n";
        s += "Potrzebne zasoby - brakuje: " + "\n";
        if (MissingResourcesCount > 0)
            foreach (int resourceID in MissingResources.Keys)
            {
                s += "- " + GameManager.Instance.World.ResourcesInfo[resourceID].Name + " - " + MissingResources[resourceID] + "\n";
            }

        s += "\n";
        s += "Potrzebne zasoby - w drodze: " + "\n";
        foreach (Character character in PendingResources.Keys)
        {
            s += "- " + character.Name + " - " + GameManager.Instance.World.ResourcesInfo[PendingResources[character]].Name + "\n";
        }
        s += "\n";
        return s;
    }
}
