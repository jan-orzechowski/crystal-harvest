using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RobotPanel : MonoBehaviour 
{
    [HideInInspector]
    public Character Character { get; protected set; }

    public ProgressBar ConditionBar;
    public Text TextSubpanel;

    void Update()
    {
        if (Character == null) return;

        ConditionBar.SetFillPercentage(1f - Character.Needs["Condition"]);
    }

    public void SetRobot(Character c)
    {
        if (c == null || c.IsRobot == false)
        {
            Character = null;
        }
        else
        {
            Character = c;
            TextSubpanel.text = c.Name;
            Update();
        }        
    }
}
