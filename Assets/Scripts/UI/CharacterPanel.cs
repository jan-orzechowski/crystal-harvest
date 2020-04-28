using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterPanel : MonoBehaviour 
{
    public SelectionPanel SelectionPanel;
    public Character Character { get; protected set; }

    public ProgressBar HealthBar;
    public ProgressBar HungerBar;

    public Text TextSubpanel;

    public Text HealthText;
    public Text HungerText;

    void Update()
    {
        if (Character == null 
            || Character.DisplayObject == null 
            || Character.DisplayObject.DeathAnimationStarted)
        {
            SelectionPanel.RemoveSelection();
            Character = null;
            return;
        }

        HealthBar.SetFillPercentage(1f - Character.Needs["Health"]);
        HungerBar.SetFillPercentage(1f - Character.Needs["Hunger"]);
    }

    public void SetCharacter(Character c)
    {
        HealthText.text = GameManager.Instance.TextManager.GetText("Health");
        HungerText.text = GameManager.Instance.TextManager.GetText("Hunger");

        if (c == null || c.IsRobot)
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
