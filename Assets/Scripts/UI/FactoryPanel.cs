using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FactoryPanel : MonoBehaviour 
{    
    [HideInInspector]
    public Factory Factory { get; protected set; }

    public ProgressBar ProgressBar;

    List<ResourceIconSlot> inputSlots;
    List<ResourceIconSlot> outputSlots;

    List<GameObject> icons;

    List<int> tempRequiredResources;  
    List<int> tempResources;

    void Awake()
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();

        if (slotsArray == null || slotsArray.Length == 0)
        {
            Debug.Log("Panel nie ma żadnych slotów na zasoby!");
        }

        inputSlots = new List<ResourceIconSlot>();
        outputSlots = new List<ResourceIconSlot>();

        for (int i = 0; i < slotsArray.Length; i++)
        {
            if (slotsArray[i].Type == ResourceIconSlotType.Input)
            {
                inputSlots.Add(slotsArray[i]);
            }
            else if (slotsArray[i].Type == ResourceIconSlotType.Output)
            {
                outputSlots.Add(slotsArray[i]);
            }
        }

        inputSlots.OrderBy(slot => slot.Order);
        outputSlots.OrderBy(slot => slot.Order);

        icons = new List<GameObject>();

        tempRequiredResources = new List<int>();
        tempResources = new List<int>();
    }

    void Update()
    {
        if (Factory == null) return;

        ProgressBar.SetFillPercentage(Factory.GetCompletionPercentage());

        SelectionPanel.HideResourceIcons(icons);

        if (Factory.Prototype.ConsumedResources != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(Factory.Prototype.ConsumedResources);
            tempRequiredResources.Sort();
            tempResources = SelectionPanel.GetResourcesList(Factory.InputStorage.Resources);
            SelectionPanel.ShowIconsWithRequirements(inputSlots, tempRequiredResources, tempResources, icons);
        }

        if (Factory.Prototype.ProducedResources != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(Factory.Prototype.ProducedResources);    
            tempRequiredResources.Sort();

            tempResources = SelectionPanel.GetResourcesList(Factory.OutputStorage.ResourcesToRemove);           
            foreach (Character c in Factory.OutputStorage.ReservedResources.Keys)
            {
                tempResources.Add(Factory.OutputStorage.ReservedResources[c]);
            }

            SelectionPanel.ShowIconsWithRequirements(outputSlots, tempRequiredResources, tempResources, icons);
        }
    }

    public void SetFactory(Factory f)
    {
        Factory = f;
    }
}
