using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class NaturalDepositPanel : MonoBehaviour 
{
    [HideInInspector]
    public Factory Deposit;

    public ProgressBar ProgressBar;
    public Text TextSubpanel;

    void Update()
    {
        if (Deposit == null) return;

        ProgressBar.SetFillPercentageWithoutText(Deposit.GetRemainingProductionCyclesPercentage());
        ProgressBar.SetText(Deposit.RemainingProductionCycles + " / " + Deposit.StartingProductionCycles);
    }

    public void SetDeposit(Factory f)
    {
        if (f == null || f.IsNaturalDeposit == false)
        {
            Deposit = null;
        }
        else
        {
            Deposit = f;
            if (f != null && f.Building != null) TextSubpanel.text = f.Building.Type;
        }      
    }
}
