using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NaturalDepositDisplay : SelectableDisplayObject 
{
    public GameObject Model;
    public GameObject DepletedModel;

    Factory deposit;

    void Update () 
    {
        if (deposit == null) { return; }

        if (deposit.RemainingProductionCycles <= 0)
        {
            if (Model.activeSelf) Model.SetActive(false);
            if (DepletedModel.activeSelf == false) DepletedModel.SetActive(true);
        }
        else
        {
            if (Model.activeSelf == false) Model.SetActive(true);
            if (DepletedModel.activeSelf) DepletedModel.SetActive(false);
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
