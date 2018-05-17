using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoragePanel : MonoBehaviour 
{
    public SelectionPanel SelectionPanel;

    public Storage Storage { get; protected set; }

    public StorageSubpanel TwoRowsSubpanel;
    public StorageSubpanel ThreeRowsSubpanel;
    public StorageSubpanel FourRowsSubpanel;

    StorageSubpanel activePanel;

    List<int> tempResourcesList;
    
    void Awake()
    {
        tempResourcesList = new List<int>();
    }

    void Update()
    {
        if (Storage == null || Storage.Building.IsDeconstructed)
        {
            Storage = null;
            SelectionPanel.RemoveSelection();
            return;
        }
    
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

        if (TwoRowsSubpanel.gameObject.activeSelf) TwoRowsSubpanel.gameObject.SetActive(false);
        if (ThreeRowsSubpanel.gameObject.activeSelf) ThreeRowsSubpanel.gameObject.SetActive(false);
        if (FourRowsSubpanel.gameObject.activeSelf) FourRowsSubpanel.gameObject.SetActive(false);

        if (Storage != null)
        {
            if (Storage.MaxCapacity <= 12) activePanel = TwoRowsSubpanel;
            else if (Storage.MaxCapacity <= 18) activePanel = ThreeRowsSubpanel;
            else activePanel = FourRowsSubpanel;

            activePanel.TextSubpanel.text = Storage.Building.Name;
           
            activePanel.gameObject.SetActive(true);

            ShowResourcesInStorage();
        }
    }

    public void DeconstructButtonAction()
    {
        if (Storage == null) return;
        GameManager.Instance.World.MarkBuildingForDenconstruction(Storage.Building);
    }

    public void CancelDeconstructionButtonAction()
    {
        if (Storage == null) return;
        GameManager.Instance.World.CancelBuildingDeconstruction(Storage.Building);        
    }
}
