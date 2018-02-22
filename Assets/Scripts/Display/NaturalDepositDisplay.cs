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
            if (Model.activeSelf) Model.SetActive(false);
            if (DepletedModel.activeSelf == false) DepletedModel.SetActive(true);
        }
    }

    public void AssignDeposit(Factory deposit)
    {
        this.deposit = deposit;
        AssignModelObject((ISelectable)deposit.Building);
    }
}
