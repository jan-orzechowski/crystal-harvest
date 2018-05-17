using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    public InputManager InputManager;

    public ServicePanel ServicePanel;
    public FactoryPanel FactoryPanel;
    public NaturalDepositPanel NaturalDepositPanel;
    public StoragePanel StoragePanel;
    public ConstructionPanel ConstructionPanel;
    public CharacterPanel CharacterPanel;
    public RobotPanel RobotPanel;
    public BuildingModulePanel BuildingModulePanel;
    public OtherBuildingPanel OtherBuildingPanel;

    public bool PanelVisible { get; protected set; }

    void Start()
    {
        HidePanels();
    }

    public void AssignSelectedObject(ISelectable obj)
    {
        HidePanels();

        if (obj is Building)
        {
            PanelVisible = true;

            Building building = (Building)obj;
            
            if (building.Module == null)
            {               
                if (building.GetDisplayObject() is ConstructionSiteDisplayObject)
                {
                    ConstructionSiteDisplayObject cds = (ConstructionSiteDisplayObject)building.GetDisplayObject();
                    ConstructionPanel.gameObject.SetActive(true);
                    ConstructionPanel.SetConstructionSite(cds.ConstructionSite);

                    BuildingModulePanel.gameObject.SetActive(true);
                    BuildingModulePanel.SetModule(cds.ConstructionSite);
                }
                else if (building.Type == "Stairs" 
                         || building.Type == "Platform"
                         || building.Type == "Slab")
                {
                    OtherBuildingPanel.gameObject.SetActive(true);
                    OtherBuildingPanel.SetOtherBuilding(building);
                }
            }
            else
            {                
                if (building.Module is Storage)
                {
                    StoragePanel.gameObject.SetActive(true);
                    StoragePanel.SetStorage((Storage)building.Module);
                }
                else if (building.Module is Factory)
                {
                    if (((Factory)building.Module).IsNaturalDeposit)
                    {
                        NaturalDepositPanel.gameObject.SetActive(true);
                        NaturalDepositPanel.SetDeposit((Factory)building.Module);
                    }
                    else
                    {
                        FactoryPanel.gameObject.SetActive(true);
                        FactoryPanel.SetFactory((Factory)building.Module);
                    }
                }
                else if (building.Module is Service)
                {
                    ServicePanel.gameObject.SetActive(true);
                    ServicePanel.SetService((Service)building.Module);
                }

                if (building.Prototype.IsNaturalDeposit) return;
                
                BuildingModulePanel.gameObject.SetActive(true);
                BuildingModulePanel.SetModule(building.Module);                
            }
        }
        else
        if (obj is Character)
        {
            PanelVisible = true;

            Character c = (Character)obj;
            if (c.IsRobot)
            {
                RobotPanel.gameObject.SetActive(true);
                RobotPanel.SetRobot(c);
            }
            else
            {
                CharacterPanel.gameObject.SetActive(true);
                CharacterPanel.SetCharacter(c);
            }
        }
    }

    public void HidePanels()
    {
        if (PanelVisible == false) return;
        
        ServicePanel.gameObject.SetActive(false);
        ServicePanel.SetService(null);

        FactoryPanel.gameObject.SetActive(false);
        FactoryPanel.SetFactory(null);

        NaturalDepositPanel.gameObject.SetActive(false);
        NaturalDepositPanel.SetDeposit(null);

        StoragePanel.gameObject.SetActive(false);
        StoragePanel.SetStorage(null);

        ConstructionPanel.gameObject.SetActive(false);
        ConstructionPanel.SetConstructionSite(null);

        CharacterPanel.gameObject.SetActive(false);
        CharacterPanel.SetCharacter(null);
        
        RobotPanel.gameObject.SetActive(false);
        RobotPanel.SetRobot(null);

        BuildingModulePanel.gameObject.SetActive(false);
        BuildingModulePanel.SetModule(null);

        OtherBuildingPanel.gameObject.SetActive(false);
        OtherBuildingPanel.SetOtherBuilding(null);

        GameManager.Instance.Tooltip.Hide();

        PanelVisible = false;
    }

    public void RemoveSelection()
    {
        HidePanels();
        InputManager.RemoveSelection();
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

    public static List<int> GetResourcesList(Dictionary<Character, int> resources)
    {
        if (resources == null) { return null; }

        List<int> result = new List<int>();
        foreach (Character c in resources.Keys)
        {
            result.Add(resources[c]);
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

        if (rdi == null)
        {
            Debug.LogWarning("Brakuje ResourceDisplayInfo dla zasobu: " + resourceID);
            return;
        }

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

    public static void HideResourceIcons(List<GameObject> icons, Transform hiddenIconsParent)
    {
        foreach (GameObject icon in icons)
        {
            icon.transform.SetParent(hiddenIconsParent);
            SimplePool.Despawn(icon);
        }
        icons.Clear();
    }
}
