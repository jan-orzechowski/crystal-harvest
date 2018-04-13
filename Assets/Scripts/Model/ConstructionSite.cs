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

    StorageWithRequirements ConstructionStorage;
    Storage DeconstructionStorage;

    public StorageWithRequirements InputStorage { get { return ConstructionStorage;  } }
    public Storage OutputStorage { get { return DeconstructionStorage; } }

    float constructionTime;
    
    float stageTimeLeft;
    static float scaffoldingStageTime = 5f;

    public ConstructionStage Stage { get; protected set; }

    public bool ConstructionMode   { get { return (Stage == ConstructionStage.Construction 
                                            || Stage == ConstructionStage.ScaffoldingConstruction);  } }
    public bool DeconstructionMode { get { return (Stage == ConstructionStage.Deconstruction 
                                            || Stage == ConstructionStage.ScaffoldingDeconstruction);  } }

    public Character WorkingCharacter { get; protected set; }
    Character jobReservation;
    float jobReservationTimer;
    float timeWithoutWork;

    public BuildingPrototype Prototype { get; protected set; }
    World world;

    public bool HidesCharacter { get { return false; } }

    public bool Halted { get; protected set; }
    public bool CanAbort { get; protected set; }
    public bool TransitionToDeconstructionStage { get; protected set; }

    public ConstructionSite(Building building, BuildingPrototype buildingPrototype, bool deconstruction)
    {
        Building = building;
        Prototype = buildingPrototype;
        world = GameManager.Instance.World;

        constructionTime = Prototype.ConstructionTime;

        Halted = false;

        if (deconstruction)
        {
            CanAbort = false;
            Stage = ConstructionStage.Deconstruction;                         
            stageTimeLeft = constructionTime;          
        }
        else
        {
            CanAbort = true;

            if (Prototype.ConstructionWithoutScaffolding)
            {
                Stage = ConstructionStage.Construction;
                LoadRequiredResourcesForConstruction();
                stageTimeLeft = constructionTime;
            }
            else
            {
                Stage = ConstructionStage.ScaffoldingConstruction;
                LoadRequiredResourcesForScaffoldingConstruction();
                stageTimeLeft = scaffoldingStageTime;
            }
        }                
    }

    public void UpdateConstructionSite(float deltaTime)
    {
        if (TransitionToDeconstructionStage)
        {
            if (ConstructionStorage.IsWaitingForResources() == false)
            {
                DeconstructionStorage = new Storage(Building, ConstructionStorage.Resources, true);
                ConstructionStorage = new StorageWithRequirements(Building, null);

                if (Stage == ConstructionStage.Construction)
                {
                    Stage = ConstructionStage.Deconstruction;
                    stageTimeLeft = scaffoldingStageTime - stageTimeLeft;
                }
                else if (Stage == ConstructionStage.ScaffoldingConstruction)
                {
                    Stage = ConstructionStage.ScaffoldingDeconstruction;
                    stageTimeLeft = constructionTime - stageTimeLeft;
                }

                TransitionToDeconstructionStage = false;
            }

            return;
        }


        if (WorkingCharacter == null)
        {
            jobReservationTimer -= deltaTime;
            if (jobReservationTimer < 0f)
            {
                jobReservation = null;
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
        if (Halted || TransitionToDeconstructionStage) return false;

        if (ConstructionMode && ConstructionStorage.AreRequirementsMet == false)
        {
            return false;
        }
        else if (DeconstructionMode && DeconstructionStorage.IsEmpty == false)
        {
            return false;
        }
       
        if (WorkingCharacter != null && WorkingCharacter != workingCharacter)
        {
            return false;
        }

        jobReservation = null;

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
                LoadRequiredResourcesForConstruction();
            }
            else if (Stage == ConstructionStage.Construction)
            {
                world.UnregisterResources(InputStorage.Resources);
                world.FinishConstruction(this);
            }
            else if (Stage == ConstructionStage.Deconstruction)
            {
                if (Prototype.ConstructionWithoutScaffolding)
                {
                    world.FinishDeconstruction(this);
                }
                else
                {
                    Stage = ConstructionStage.ScaffoldingDeconstruction;
                    LoadResourcesFromDeconstruction();
                    stageTimeLeft = scaffoldingStageTime;
                }                
            }
            else if (Stage == ConstructionStage.ScaffoldingDeconstruction)
            {
                world.FinishDeconstruction(this);
            }
        }

        return true;        
    }

    public void CancelConstruction()
    {
        if (Stage == ConstructionStage.Construction || Stage == ConstructionStage.ScaffoldingConstruction)
        {
            TransitionToDeconstructionStage = true;
            Halted = false;
            CanAbort = false;
        }
        else
        {
            Debug.LogWarning("Próbujemy przerwać budowę, kiedy trwa dekonstrukcja budynku");
        }        
    }

    public void SetHaltStatus(bool halted)
    {
        Halted = halted;
    }

    public bool CanReserveJob(Character character)
    {
        return (Halted == false
                && TransitionToDeconstructionStage == false
                && WorkingCharacter == null
                && (jobReservation == null || jobReservation == character)
                && ((ConstructionMode && ConstructionStorage.AreRequirementsMet)
                    || DeconstructionMode && DeconstructionStorage.IsEmpty));
    }

    public bool ReserveJob(Character character)
    {
        if (CanReserveJob(character))
        {
            jobReservation = character;
            jobReservationTimer = 5f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RenewJobReservation(Character character)
    {
        if (jobReservation == character)
        {
            jobReservationTimer = 5f;
        }
    }

    void LoadRequiredResourcesForConstruction()
    {      
        ConstructionStorage = new StorageWithRequirements(Building, Prototype.ConstructionResources);
        DeconstructionStorage = new Storage(Building, null);
    }

    void LoadRequiredResourcesForScaffoldingConstruction()
    {
        ConstructionStorage = new StorageWithRequirements(Building, Prototype.ResourcesForScaffoldingConstruction);
        DeconstructionStorage = new Storage(Building, null);
    }

    void LoadResourcesFromDeconstruction()
    {
        ConstructionStorage = new StorageWithRequirements(Building, null);
        DeconstructionStorage = new Storage(Building, Prototype.ResourcesFromDeconstruction, true);
        GameManager.Instance.World.RegisterResources(Prototype.ResourcesFromDeconstruction);
    }   

    public float GetCompletionPercentage()
    {
        float result;

        if (Prototype.ConstructionWithoutScaffolding)
        {
            result = (constructionTime - stageTimeLeft) / constructionTime;
            return result;
        }

        float totalTime = constructionTime + scaffoldingStageTime;
        if (ConstructionMode)
        {
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
        if (Stage == ConstructionStage.ScaffoldingConstruction 
            || Stage == ConstructionStage.ScaffoldingDeconstruction)        
        {
            result = (scaffoldingStageTime - stageTimeLeft) / scaffoldingStageTime;
        }
        else
        {
            result = (constructionTime - stageTimeLeft) / constructionTime;
        }        
        return result;
    }
    
    public Tile GetAccessTile()
    {
        if (Building != null)
            return Building.GetAccessTile();
        else return null;
    }

    public Rotation GetAccessTileRotation()
    {
        if (Building != null)
            return Building.GetAccessTileRotation();
        else return Rotation.N;
    }
  
    public string GetSelectionText()
    {
        string s = "";

        s += "Wstrzymane: " + Halted.ToString() + "\n";

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
