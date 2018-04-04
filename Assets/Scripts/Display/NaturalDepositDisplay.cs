using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NaturalDepositDisplay : SelectableDisplayObject 
{
    public GameObject Model;
    public GameObject DepletedModel;

    public BoxCollider DepletedCollider;

    Factory deposit;

    bool resourcesDepleted;

    void Awake()
    {
        Model.SetActive(true);
        DepletedModel.SetActive(false);
        
        if (DepletedCollider == null)
        {
            DepletedCollider = Collider;
        }
        else
        {
            DepletedCollider.gameObject.SetActive(false);
        }

        Collider.gameObject.SetActive(true);
    }

    void Update () 
    {
        if (deposit == null) { return; }

        if (deposit.RemainingProductionCycles <= 0)
        {
            if (resourcesDepleted == false)
            {
                Model.SetActive(false);
                DepletedModel.SetActive(true);

                Collider.gameObject.SetActive(false);
                DepletedCollider.gameObject.SetActive(true);

                Collider = DepletedCollider;
                resourcesDepleted = true;
            }
        }
    }

    public override void AssignModelObject(ISelectable modelObject)
    {
        base.AssignModelObject(modelObject);

        if (modelObject != null && modelObject is Building)
        {
            Building building = (Building)modelObject;
            if (building.Module != null && building.Module is Factory)
            {
                deposit = (Factory)building.Module;
                return;
            }
        }

        Debug.LogWarning("Stworzono instancję NaturalDepositDisplay bez przypisanej fabryki");
    }
}
