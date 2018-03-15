using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ConstructionPanel : MonoBehaviour 
{
    [HideInInspector]
    public ConstructionSite ConstructionSite;

    public ProgressBar ProgressBar;

    List<ResourceIconSlot> slots;

    List<GameObject> icons;

    List<int> tempRequiredResources;
    List<int> tempResources;

    public InputManager InputManager;

    void Start ()
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();
        if (slotsArray == null || slotsArray.Length == 0)
        {
            Debug.Log("Panel nie ma żadnych slotów na zasoby!");
        }
        slots = new List<ResourceIconSlot>(slotsArray);
        slots.OrderBy(slot => slot.Order);
       
        icons = new List<GameObject>();

        tempRequiredResources = new List<int>();
        tempResources = new List<int>();
    }

    void Update()
    {
        if (ConstructionSite == null)
        {
            return;
        }
        else 
        if ((ConstructionSite.ConstructionMode && ConstructionSite.GetStageCompletionPercentage() >= 1f)
             || (ConstructionSite.DeconstructionMode && ConstructionSite.GetStageCompletionPercentage() <= 0f))
        {
            ConstructionSite = null;
            InputManager.RemoveSelection();
            return;
        }     

        ProgressBar.SetFillPercentage(ConstructionSite.GetStageCompletionPercentage());

        SelectionPanel.HideResourceIcons(icons);

        if (ConstructionSite.ConstructionMode)
        {
            if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction
                && ConstructionSite.Prototype.ResourcesForScaffoldingConstruction != null)
            {
                tempRequiredResources = SelectionPanel.GetResourcesList(ConstructionSite.Prototype.ResourcesForScaffoldingConstruction);
                tempRequiredResources.Sort();
                tempResources = SelectionPanel.GetResourcesList(ConstructionSite.InputStorage.Resources);
                SelectionPanel.ShowIconsWithRequirements(slots, tempRequiredResources, tempResources, icons);
            }
            else if (ConstructionSite.Prototype.ConstructionResources != null)
            {
                tempRequiredResources = SelectionPanel.GetResourcesList(ConstructionSite.Prototype.ConstructionResources);
                tempRequiredResources.Sort();
                tempResources = SelectionPanel.GetResourcesList(ConstructionSite.InputStorage.Resources);
                SelectionPanel.ShowIconsWithRequirements(slots, tempRequiredResources, tempResources, icons);
            }           
        }
        else 
        if (ConstructionSite.DeconstructionMode
            && ConstructionSite.Stage != ConstructionStage.ScaffoldingDeconstruction
            && ConstructionSite.Prototype.ResourcesFromDeconstruction != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(ConstructionSite.Prototype.ResourcesFromDeconstruction);
            tempRequiredResources.Sort();

            tempResources = SelectionPanel.GetResourcesList(ConstructionSite.OutputStorage.ResourcesToRemove);
            foreach (Character c in ConstructionSite.OutputStorage.ReservedResources.Keys)
            {
                tempResources.Add(ConstructionSite.OutputStorage.ReservedResources[c]);
            }

            SelectionPanel.ShowIconsWithRequirements(slots, tempRequiredResources, tempResources, icons);            
        }
    }

    public void SetConstructionSite(ConstructionSite cs)
    {
        ConstructionSite = cs;
    }
}
