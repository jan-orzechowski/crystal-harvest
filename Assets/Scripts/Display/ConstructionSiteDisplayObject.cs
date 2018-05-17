using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConstructionSiteDisplayObject : SelectableDisplayObject
{
    public ConstructionSite ConstructionSite;

    public GameObject ConstructionPlane;
    public GameObject Scaffolding;
    public GameObject ClippableScaffolding;
    public GameObject Building;
    public GameObject ClippableBuilding;

    public bool TwoLevelBuilding = false;
    public float HeightOffset = 0f;
    
    float minColliderHeight = 0.1f;

    float scaffoldingHeight = 1.2f;
    float scaffoldingColliderHeightDifference = 0.05f;

    bool constructionWithoutScaffolding;

    GameObject currentBuilding;
    GameObject currentScaffolding;

    float levelOffset;

    float colliderBottomPlane;
    
    float startingScaffoldingUpperPlanePosition;
    float finalScaffoldingUpperPlanePosition;

    float startingBuildingYPosition;
    float finalBuildingYPosition;

    public float OverrideUpperPlanePosition = 0f;

    void Awake()
    {
        if (ClippableBuilding == null || Building == null)
        {
            Debug.LogError("Brak modelu budynku");     
        }
       
        if (ClippableScaffolding == null || Scaffolding == null)
        {
            Debug.LogError("Brak modelu rusztowania");
        }
    }

    void Update()
    {
        if (ConstructionSite == null) return;
        
        if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction)
        {
            float y = Mathf.Lerp(startingScaffoldingUpperPlanePosition,
                                 finalScaffoldingUpperPlanePosition, 
                                 ConstructionSite.GetStageCompletionPercentage());

            SetScaffoldingUpperPlanePosition(y);
            SetColliderUpperPlanePosition(y - scaffoldingColliderHeightDifference);
        }
        else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingDeconstruction)
        {
            float y = Mathf.Lerp(finalScaffoldingUpperPlanePosition,
                                 startingScaffoldingUpperPlanePosition,                                 
                                 ConstructionSite.GetStageCompletionPercentage());

            SetScaffoldingUpperPlanePosition(y);
            SetColliderUpperPlanePosition(y - scaffoldingColliderHeightDifference);
        }
        else if (ConstructionSite.Stage == ConstructionStage.Construction)
        {
            float y = Mathf.Lerp(startingBuildingYPosition,
                                 finalBuildingYPosition,
                                 ConstructionSite.GetStageCompletionPercentage());

            SetBuilidingYPosition(y);

            if (constructionWithoutScaffolding)
            {
                SetColliderUpperPlanePosition(OverrideUpperPlanePosition + y);
            }
            else
            if (ConstructionSite.TransitionToDeconstructionStage)
            {
                SetScaffoldingUpperPlanePosition(finalScaffoldingUpperPlanePosition);
                SetColliderUpperPlanePosition(finalScaffoldingUpperPlanePosition - scaffoldingColliderHeightDifference);
            }
        }
        else if (ConstructionSite.Stage == ConstructionStage.Deconstruction)
        {
            float y = Mathf.Lerp(finalBuildingYPosition,
                                 startingBuildingYPosition,
                                 ConstructionSite.GetStageCompletionPercentage());
            SetBuilidingYPosition(y);

            if (constructionWithoutScaffolding == false)
            {
                SetScaffoldingUpperPlanePosition(finalScaffoldingUpperPlanePosition);
                SetColliderUpperPlanePosition(finalScaffoldingUpperPlanePosition - scaffoldingColliderHeightDifference);
            }
            else
            {
                SetColliderUpperPlanePosition(OverrideUpperPlanePosition + y);
            }               
        }      
    }

    void SetBuilidingYPosition(float y)
    {        
        currentBuilding.transform.SetPositionAndRotation(
            new Vector3(currentBuilding.transform.position.x, y, currentBuilding.transform.position.z),
            currentBuilding.transform.rotation);
    }

    void SetScaffoldingUpperPlanePosition(float y)
    {
        float newScaffoldingYPosition = y - scaffoldingHeight;
        if (TwoLevelBuilding) newScaffoldingYPosition -= scaffoldingHeight;

        currentScaffolding.transform.SetPositionAndRotation(
            new Vector3(currentScaffolding.transform.position.x, 
                        newScaffoldingYPosition, 
                        currentScaffolding.transform.position.z),
            currentScaffolding.transform.rotation);    
    }

    void SetColliderUpperPlanePosition(float y)
    {
        float newColliderHeight = y - colliderBottomPlane; 

        if (newColliderHeight < minColliderHeight) newColliderHeight = minColliderHeight;

        float newSizeY = newColliderHeight;
        float newCenterY = newColliderHeight / 2;

        Collider.size = new Vector3(Collider.size.x, newSizeY, Collider.size.z);
        Collider.center = new Vector3(Collider.center.x, newCenterY, Collider.center.z);
    }

    public void AssignConstructionSite(ConstructionSite constructionSite, bool builtOnSecondLevel)
    {
        ConstructionSite = constructionSite;
        ModelObject = constructionSite.Building;

        Scaffolding.SetActive(false);
        Building.SetActive(false);
        ClippableBuilding.SetActive(false);
        ClippableScaffolding.SetActive(false);

        if (builtOnSecondLevel)
        {
            currentBuilding = ClippableBuilding;
            currentScaffolding = ClippableScaffolding;
        }
        else
        {
            currentBuilding = Building;
            currentScaffolding = Scaffolding;
        }

        currentBuilding.SetActive(true);
        currentScaffolding.SetActive(true);

        constructionWithoutScaffolding = ConstructionSite.Building.Prototype.ConstructionWithoutScaffolding;

        levelOffset = builtOnSecondLevel ? 1f : 0f;
       
        colliderBottomPlane = levelOffset + 0.005f;

        startingScaffoldingUpperPlanePosition = levelOffset - 0.1f;
        finalScaffoldingUpperPlanePosition = levelOffset + scaffoldingHeight;

        startingBuildingYPosition = levelOffset - HeightOffset - 1.2f;
        finalBuildingYPosition = levelOffset;

        if (TwoLevelBuilding)
        {
            startingBuildingYPosition -= 1.3f;
            startingScaffoldingUpperPlanePosition -= 1.3f;
            finalScaffoldingUpperPlanePosition += scaffoldingHeight;
        }

        if (OverrideUpperPlanePosition > 0.05f)
        {
            finalScaffoldingUpperPlanePosition = OverrideUpperPlanePosition;
        }

        SetScaffoldingUpperPlanePosition(startingScaffoldingUpperPlanePosition);
        SetBuilidingYPosition(startingBuildingYPosition);

        Collider.transform.SetPositionAndRotation(
            new Vector3(Collider.transform.position.x,
                        levelOffset,
                        Collider.transform.position.z),
            Collider.transform.rotation);

        SetColliderUpperPlanePosition(levelOffset);
    }
}