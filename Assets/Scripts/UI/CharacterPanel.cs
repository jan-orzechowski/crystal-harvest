using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterPanel : MonoBehaviour 
{
    [HideInInspector]
    public Character Character { get; protected set; }

    public ProgressBar HealthBar;
    public ProgressBar HungerBar;
    public Text TextSubpanel;

    void Update()
    {
        if (Character == null) return;

        HealthBar.SetFillPercentage(Character.Needs["Health"]);
        HungerBar.SetFillPercentage(Character.Needs["Hunger"]);
    }

    public void SetCharacter(Character c)
    {
        if (c == null || c.IsRobot)
        {
            Character = null;
        }
        else
        {
            Character = c;
            TextSubpanel.text = c.Name;
        }      
    }
}
