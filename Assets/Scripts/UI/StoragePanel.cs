using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoragePanel : MonoBehaviour 
{
    [HideInInspector]
    public Storage Storage { get; protected set; }

    public StorageSubpanel TwoRowsSubpanel;
    public StorageSubpanel ThreeRowsSubpanel;
    public StorageSubpanel FourRowsSubpanel;

    StorageSubpanel activePanel;

    List<int> tempResourcesList;

    public GameObject DeconstructButton;

    void Awake()
    {
        tempResourcesList = new List<int>();
    }

    void Update()
    {
        if (Storage == null) return;

        if (Storage.Changed == false)
        {
            return;
        }
        else
        {
            Storage.Changed = false;
            ShowResourcesInStorage();
        }
    }

    public void ShowResourcesInStorage()
    {
        if (activePanel == null || activePanel.gameObject.activeSelf == false) return;

        tempResourcesList = SelectionPanel.GetResourcesList(Storage.Resources);
        tempResourcesList.AddRange(SelectionPanel.GetResourcesList(Storage.ReservedResources));        
        tempResourcesList.Sort();

        activePanel.HideResourceIcons();

        for (int index = 0; index < tempResourcesList.Count; index++)
        {
            activePanel.ShowResourceIcon(tempResourcesList[index], index);
        }
    }

    public void SetStorage(Storage s)
    {
        Storage = s;

        TwoRowsSubpanel.gameObject.SetActive(false);
        ThreeRowsSubpanel.gameObject.SetActive(false);
        FourRowsSubpanel.gameObject.SetActive(false);

        if (Storage != null)
        {
            if (Storage.MaxCapacity <= 12) activePanel = TwoRowsSubpanel;
            else if (Storage.MaxCapacity <= 18) activePanel = ThreeRowsSubpanel;
            else activePanel = FourRowsSubpanel;

            activePanel.gameObject.SetActive(true);

            activePanel.TextSubpanel.text = Storage.Building.Type;

            ShowResourcesInStorage();
        }
    }

    public void DeconstructButtonAction()
    {
        if (Storage == null) return;
        GameManager.Instance.World.MarkBuildingToDenconstruction(Storage.Building);
        SetStorage(null);
        DeconstructButton.SetActive(false);
    }
}
