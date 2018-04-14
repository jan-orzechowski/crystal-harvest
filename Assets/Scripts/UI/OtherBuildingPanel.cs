using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OtherBuildingPanel : MonoBehaviour 
{
    public SelectionPanel SelectionPanel;

    Building building;
    public Text Text;

    public GameObject DeconstructButton;
    
    void Update()
    {
        if (building == null || building.IsDeconstructed)
        {
            building = null;
            SelectionPanel.RemoveSelection();
        }    
    }

    public void SetOtherBuilding(Building b)
    {
        building = b;
        if (building != null)
        {
            Text.text = building.Type;
        }        
    }

    public void DeconstructButtonAction()
    {
        if (building != null)
        {
            GameManager.Instance.World.MarkBuildingForDenconstruction(building);
            building = null;
        }        
    }
}
