using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Text Text;
    public Text SubText;

    public GameObject StartButton;
    public GameObject StopButton;
    public GameObject AbortButton;

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

        AbortButton.SetActive(true);
        StopButton.SetActive(false);
        StartButton.SetActive(false);
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
            InputManager.RemoveSelection();            
            return;
        }

        if (ConstructionSite.Building != null)
        {
            Text.text = ConstructionSite.Building.Type;

            if (ConstructionSite.Stage == ConstructionStage.Construction)
                SubText.text = "Plac budowy";
            else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingConstruction)
                SubText.text = "Budowa rusztowania";
            else if (ConstructionSite.Stage == ConstructionStage.Deconstruction)
                SubText.text = "Rozbiórka";
            else if (ConstructionSite.Stage == ConstructionStage.ScaffoldingDeconstruction)
                SubText.text = "Rozbiórka rusztowania";
        }

        float percentage = ConstructionSite.GetStageCompletionPercentage();

        if (ConstructionSite.DeconstructionMode) percentage = 1f - percentage;

        if (ConstructionSite.Halted)
        {
            ProgressBar.SetFillPercentageWithoutText(percentage);
            ProgressBar.SetText("Wstrzymane");
        }
        else
        {
            ProgressBar.SetFillPercentage(percentage);
        }

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

    public void SetConstructionSite(ConstructionSite cs)
    {
        ConstructionSite = cs;
        if (cs != null)
        {            
            AbortButton.SetActive(cs.CanAbort);           

            if (cs.Halted)
            {
                StartButton.SetActive(true);
                StopButton.SetActive(false);
            }
            else
            {
                StartButton.SetActive(false);
                StopButton.SetActive(true);
            }
        } 
    }

    public void AbortButtonAction()
    {
        if (ConstructionSite == null) return;
        ConstructionSite.CancelConstruction();
        AbortButton.SetActive(false);
    }

    public void StopButtonAction()
    {
        if (ConstructionSite == null) return;
        ConstructionSite.SetHaltStatus(true);
        StopButton.SetActive(false);
        StartButton.SetActive(true);
    }

    public void StartButtonAction()
    {
        if (ConstructionSite == null) return;
        ConstructionSite.SetHaltStatus(false);
        StopButton.SetActive(true);
        StartButton.SetActive(false);
    }
}
