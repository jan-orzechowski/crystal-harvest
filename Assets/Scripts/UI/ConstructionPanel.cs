using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class ConstructionPanel : MonoBehaviour 
{
    public SelectionPanel SelectionPanel;

    public ConstructionSite ConstructionSite { get; protected set; }
   
    public ProgressBar ProgressBar;

    List<ResourceIconSlot> slots;

    List<GameObject> icons;

    List<int> tempRequiredResources;
    List<int> tempResources;

    public Text Text;
    public Text SubText;

    bool firstShow;
    ConstructionStage lastStage;
    bool lastTransitionState;

    void Awake ()
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();

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
        if (ConstructionSite.GetCompletionPercentage() >= 1f)
        {
            ConstructionSite = null;
            SelectionPanel.RemoveSelection();            
            return;
        }

        if (ConstructionSite.Building != null)
        {            
            if (firstShow 
                || lastStage != ConstructionSite.Stage 
                || lastTransitionState != ConstructionSite.TransitionToDeconstructionStage)
            {
                if (ConstructionSite.TransitionToDeconstructionStage)
                {
                    if (ConstructionSite.Stage == ConstructionStage.Construction)
                        Text.text = GameManager.Instance.GetText("s_deconstruction_site");
                    else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction)
                        Text.text = GameManager.Instance.GetText("s_scaffolding_deconstruction");
                }
                else
                {
                    if (ConstructionSite.Stage == ConstructionStage.Construction)
                        Text.text = GameManager.Instance.GetText("s_construction_site");
                    else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction)
                        Text.text = GameManager.Instance.GetText("s_scaffolding_construction");
                    else if (ConstructionSite.Stage == ConstructionStage.Deconstruction)
                        Text.text = GameManager.Instance.GetText("s_deconstruction_site");
                    else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingDeconstruction)
                        Text.text = GameManager.Instance.GetText("s_scaffolding_deconstruction");
                }
            }           

            SubText.text = ConstructionSite.Building.Name;
        }

        float percentage = ConstructionSite.GetStageCompletionPercentage();

        if (ConstructionSite.DeconstructionMode
            || (ConstructionSite.ConstructionMode 
                && ConstructionSite.TransitionToDeconstructionStage))
        {
            percentage = 1f - percentage;
        }

        if (ConstructionSite.Halted)
        {
            ProgressBar.SetFillPercentageWithoutText(percentage);
            ProgressBar.SetText(GameManager.Instance.GetText("s_halted"));
        }
        else
        {
            ProgressBar.SetFillPercentage(percentage);
        }

        if (firstShow 
            || ConstructionSite.InputStorage.Changed
            || ConstructionSite.OutputStorage.Changed
            || lastStage != ConstructionSite.Stage
            || lastTransitionState != ConstructionSite.TransitionToDeconstructionStage)
        {
            if (firstShow) firstShow = false;
            if (ConstructionSite.InputStorage.Changed) ConstructionSite.InputStorage.Changed = false;
            if (ConstructionSite.OutputStorage.Changed) ConstructionSite.OutputStorage.Changed = false;

            lastStage = ConstructionSite.Stage;
            lastTransitionState = ConstructionSite.TransitionToDeconstructionStage;

            SelectionPanel.HideResourceIcons(icons, this.transform);

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
            if (ConstructionSite.DeconstructionMode)
            {
                tempResources = SelectionPanel.GetResourcesList(ConstructionSite.OutputStorage.Resources);
                tempResources.AddRange(SelectionPanel.GetResourcesList(ConstructionSite.OutputStorage.ReservedResources));
                tempResources.Sort();

                for (int index = 0; index < tempResources.Count; index++)
                {
                    SelectionPanel.ShowResourceIcon(tempResources[index], slots[index], false, icons);
                }
            }
        }
    }

    public void SetConstructionSite(ConstructionSite cs)
    {
        ConstructionSite = cs;
        if (cs != null)
        {
            lastTransitionState = cs.TransitionToDeconstructionStage;
            firstShow = true;
            Update();
        }
    }
}
