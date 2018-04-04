using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServiceDisplayObject : SelectableDisplayObject 
{
    public GameObject CharacterModelToShow;
    bool showsCharacter;

    Service service; 

    void Update () 
    {      
        if (service.ServicedCharacter != null)
        {
            if (showsCharacter == false)
            {
                showsCharacter = true;
                CharacterModelToShow.SetActive(true);
            }
        }
        else
        {
            if (showsCharacter)
            {
                showsCharacter = false;
                CharacterModelToShow.SetActive(false);
            }
        }
    }

    public override void AssignModelObject(ISelectable modelObject)
    {
        base.AssignModelObject(modelObject);

        if (modelObject != null && modelObject is Building)
        {
            Building building = (Building)modelObject;
            if (building.Module != null && building.Module is Service)
            {
                service = (Service)building.Module;
                return;
            }
        }

        Debug.LogWarning("Stworzono instancję ServiceDisplayObject bez przypisanej usługi");
    }
}
