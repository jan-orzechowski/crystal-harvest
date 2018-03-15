using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServicePanel : MonoBehaviour 
{
    [HideInInspector]
    public Service Service;

    public ProgressBar ProgressBar;

    List<GameObject> icons;

    public ResourceIconSlot InputSlot;
    public ResourceIconSlot CharacterSlot;

    List<int> tempRequiredResources;
    List<int> tempResources;

    void Start()
    {
        icons = new List<GameObject>();
        tempRequiredResources = new List<int>();
        tempResources = new List<int>();
    }

    void Update()
    {
        if (Service == null) return;

        ProgressBar.SetFillPercentage(Service.GetServicePercentage());

        SelectionPanel.HideResourceIcons(icons);

        if (Service.Prototype.ConsumedResources != null)
        {
            tempRequiredResources = SelectionPanel.GetResourcesList(Service.Prototype.ConsumedResources);
            tempResources = SelectionPanel.GetResourcesList(Service.InputStorage.Resources);
            SelectionPanel.ShowIconsWithRequirements(new List<ResourceIconSlot>() { InputSlot }, 
                                                     tempRequiredResources, tempResources, icons);
        }

    }

    public void SetService(Service s)
    {
        Service = s;
    }
}
