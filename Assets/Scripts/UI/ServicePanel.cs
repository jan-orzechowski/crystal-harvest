using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServicePanel : MonoBehaviour
{
    [HideInInspector]
    public Service Service;

    public ProgressBar ProgressBar;

    List<GameObject> icons;

    public ResourceIconSlot InputSlot;

    List<int> tempRequiredResources;
    List<int> tempResources;

    public Text TextSubpanel;

    public GameObject DeconstructButton;

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
        if (s != null && s.Building != null) TextSubpanel.text = s.Building.Type;
    }

    public void DeconstructButtonAction()
    {
        if (Service == null) return;
        GameManager.Instance.World.MarkBuildingToDenconstruction(Service.Building);
        SetService(null);
        DeconstructButton.SetActive(false);
    }
}
