using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SelectedObjectType
{
    Storage,
    Factory,
    Service,
    ConstructionSite,
    Character
}

public class SelectionPanel : MonoBehaviour 
{
    public ServicePanel ServicePanel;
    public FactoryPanel FactoryPanel;
    public StoragePanel StoragePanel;
    public ConstructionPanel ConstructionPanel;

    void Start()
    {
        HidePanels();
    }

    public void AssignSelectedObject(ISelectable obj)
    {
        HidePanels();

        if (obj is Building)
        {
            Building building = (Building)obj;

            if (building.Module == null)
            {               
                if (building.GetDisplayObject() is ConstructionSiteDisplayObject)
                {
                    ConstructionSiteDisplayObject cds = (ConstructionSiteDisplayObject)building.GetDisplayObject();
                    ConstructionPanel.gameObject.SetActive(true);
                    ConstructionPanel.SetConstructionSite(cds.ConstructionSite);
                }
            }

            if (building.Module is Storage)
            {
                StoragePanel.gameObject.SetActive(true);
                StoragePanel.SetStorage((Storage)building.Module);
            }
            else
            if (building.Module is Factory)
            {
                FactoryPanel.gameObject.SetActive(true);
                FactoryPanel.SetFactory((Factory)building.Module);
            }
            else
            if (building.Module is Service)
            {
                ServicePanel.gameObject.SetActive(true);
                ServicePanel.SetService((Service)building.Module);
            }
        }
        else
        if (obj is Character)
        {
            Debug.Log("Selected: Character");
        }
    }

    public void HidePanels()
    {
        ServicePanel.gameObject.SetActive(false);
        ServicePanel.SetService(null);

        FactoryPanel.gameObject.SetActive(false);
        FactoryPanel.SetFactory(null);

        StoragePanel.gameObject.SetActive(false);
        StoragePanel.SetStorage(null);

        ConstructionPanel.gameObject.SetActive(false);
        ConstructionPanel.SetConstructionSite(null);
    }

    public static List<int> GetResourcesList(Dictionary<int, int> resources)
    {
        if (resources == null) { return null; }

        List<int> result = new List<int>();
        foreach (int id in resources.Keys)
        {
            for (int number = 0; number < resources[id]; number++)
            {
                result.Add(id);
            }
        }

        return result;
    }

    public static void ShowIconsWithRequirements(List<ResourceIconSlot> slots, 
                                                 List<int> requiredResources,
                                                 List<int> presentResources,
                                                 List<GameObject> icons)
    {
        if (requiredResources == null) return;
        
        // Ikony wymaganych zasobów
        for (int index = 0; index < Math.Min(requiredResources.Count, slots.Count); index++)
        {
            ShowResourceIcon(requiredResources[index], slots[index], true, icons);
        }

        List<bool> usedSlots = new List<bool>(slots.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            usedSlots.Add(false);
        }

        // Ikony obecnych zasobów
        for (int resourceIndex = 0;
            resourceIndex < presentResources.Count;
            resourceIndex++)
        {
            int resourceID = presentResources[resourceIndex];

            for (int slotIndex = 0;
                slotIndex < Math.Min(requiredResources.Count, slots.Count);
                slotIndex++)
            {
                if (requiredResources[slotIndex] == resourceID
                    && usedSlots[slotIndex] == false)
                {
                    ShowResourceIcon(resourceID, slots[slotIndex], false, icons);
                    usedSlots[slotIndex] = true;
                    break;
                }
            }
        }
    }

    public static void ShowResourceIcon(int resourceID, ResourceIconSlot slot, bool requiredIcon, List<GameObject> icons)
    {
        ResourceDisplayInfo rdi = GameManager.Instance.GetResourceDisplayInfo(resourceID);
        GameObject iconPrefab;
        if (requiredIcon)
        {
            iconPrefab = rdi.RequiredIcon;
        }
        else
        {
            iconPrefab = rdi.Icon;
        }

        GameObject icon = SimplePool.Spawn(iconPrefab,
                                           slot.transform.position,
                                           Quaternion.identity);
        icon.transform.SetParent(slot.transform);
        icons.Add(icon);
    }

    public static void HideResourceIcons(List<GameObject> icons)
    {
        foreach (GameObject icon in icons)
        {
            icon.transform.SetParent(null);
            SimplePool.Despawn(icon);
        }
        icons.Clear();
    }

}
