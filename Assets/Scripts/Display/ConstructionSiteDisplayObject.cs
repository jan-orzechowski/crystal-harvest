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

    float minColliderHeight = 0.1f;

    float colliderBottomPlane;
    float colliderMaxUpperPlane;

    bool builtOnSecondLevel = false;

    float maxY;
    float minY;

    void Awake()
    {
        if (ClippableBuilding == null && Building == null)
        {
            Debug.LogError("Brak modelu budynku");     
        }
        else if (ClippableBuilding == null) ClippableBuilding = Building;
        else if (Building == null) Building = ClippableBuilding;

        if (ClippableScaffolding == null && Scaffolding == null)
        {
            Debug.LogError("Brak rusztowania");
        }
        else if (ClippableScaffolding == null) ClippableScaffolding = Scaffolding;
        else if (Scaffolding == null) Scaffolding = ClippableScaffolding;
       
        colliderBottomPlane = Collider.transform.position.y + Collider.center.y - (Collider.size.y / 2);
        colliderMaxUpperPlane = Collider.transform.position.y + Collider.center.y + (Collider.size.y / 2);

        minY = colliderBottomPlane - colliderMaxUpperPlane - 0.21f;

        maxY = colliderBottomPlane;
    }

    void Update()
    {
        if (ConstructionSite == null) return;

        if (ConstructionPlane.activeSelf && ConstructionSite.GetCompletionPercentage() > 0.05f) ConstructionPlane.SetActive(false);

        if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction)
        {
            float y = Mathf.Lerp(minY, maxY, ConstructionSite.GetStageCompletionPercentage());
            MoveScaffolding(y);
            ResizeCollider();
        }
        else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingDeconstruction)
        {
            float y = Mathf.Lerp(maxY, minY, ConstructionSite.GetStageCompletionPercentage());
            MoveScaffolding(y);
            ResizeCollider();
        }
        else if (ConstructionSite.Stage == ConstructionStage.Construction)
        {
            float y = Mathf.Lerp(minY, maxY, ConstructionSite.GetStageCompletionPercentage() * 1.42f);
            MoveBuilding(y);
        }
        else if (ConstructionSite.Stage == ConstructionStage.Deconstruction)
        {
            float y = Mathf.Lerp(maxY, minY, ConstructionSite.GetStageCompletionPercentage());
            MoveBuilding(y);
        }      
    }

    void MoveBuilding(float y)
    {
        if (builtOnSecondLevel)
        {
            ClippableBuilding.transform.SetPositionAndRotation(
                new Vector3(ClippableBuilding.transform.position.x, y, ClippableBuilding.transform.position.z),
                ClippableBuilding.transform.rotation);
        }
        else
        {
            Building.transform.SetPositionAndRotation(
                new Vector3(Building.transform.position.x, y, Building.transform.position.z),
                Building.transform.rotation);
        }
    }

    void MoveScaffolding(float y)
    {
        if (builtOnSecondLevel)
        {
            ClippableScaffolding.transform.SetPositionAndRotation(
                new Vector3(ClippableScaffolding.transform.position.x, y, ClippableScaffolding.transform.position.z),
                ClippableScaffolding.transform.rotation);
        }
        else
        {
            Scaffolding.transform.SetPositionAndRotation(
                new Vector3(Scaffolding.transform.position.x, y, Scaffolding.transform.position.z),
                Scaffolding.transform.rotation);
        }
    }

    void ResizeCollider()
    {
        float newColliderHeight = Mathf.Lerp(0, colliderMaxUpperPlane, ConstructionSite.GetStageCompletionPercentage());
        if (newColliderHeight < minColliderHeight) { newColliderHeight = minColliderHeight; }

        float newYSize = newColliderHeight;
        float newYCenter = newColliderHeight / 2;

        Collider.size = new Vector3(Collider.size.x, newYSize, Collider.size.z);
        Collider.center = new Vector3(Collider.center.x, newYCenter, Collider.center.z);
    }

    public void AssignConstructionSite(ConstructionSite constructionSite, bool builtOnSecondLevel)
    {
        this.ConstructionSite = constructionSite;
        this.builtOnSecondLevel = builtOnSecondLevel;
        ModelObject = constructionSite.Building;

        if (builtOnSecondLevel)
        {
            Scaffolding.SetActive(false);
            Building.SetActive(false);
            ClippableBuilding.SetActive(true);
            ClippableScaffolding.SetActive(true);
        }
        else
        {
            Scaffolding.SetActive(true);
            Building.SetActive(true);
            ClippableBuilding.SetActive(false);
            ClippableScaffolding.SetActive(false);
        }

        MoveScaffolding(minY);
        MoveBuilding(minY);

        Collider.transform.position = new Vector3(
            Collider.transform.position.x, 
            constructionSite.Building.Tiles[0].Height,
            Collider.transform.position.z);
    }


}