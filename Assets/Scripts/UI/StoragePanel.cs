using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StoragePanel : MonoBehaviour 
{
    List<ResourceIconSlot> slots;

    [HideInInspector]
    public Storage Storage { get; protected set; }

    List<GameObject> icons;
    List<int> tempResourcesList;

    void Awake () 
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();
        if (slotsArray == null || slotsArray.Length == 0)
        {
            Debug.Log("Panel nie ma żadnych slotów na zasoby!");
        }
        slots = new List<ResourceIconSlot>(slotsArray);
        slots.OrderBy(slot => slot.Order);

        tempResourcesList = new List<int>();
        icons = new List<GameObject>();
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
        tempResourcesList.Clear();
        foreach (int id in Storage.Resources.Keys)
        {
            for (int number = 0; number < Storage.Resources[id]; number++)
            {
                tempResourcesList.Add(id);
            }
        }
        foreach (Character c in Storage.ReservedResources.Keys)
        {
            tempResourcesList.Add(Storage.ReservedResources[c]);
        }

        tempResourcesList.Sort();

        HideResourceIcons();

        for (int index = 0; index < tempResourcesList.Count; index++)
        {
            ShowResourceIcon(tempResourcesList[index], index);
        }
    }

    void ShowResourceIcon(int resourceID, int slotNumber)
    {
        if (slotNumber >= slots.Count)
        {
            Debug.Log("Chcemy pokazać więcej zasobów niż panel na to pozwala!");
            return;
        }

        ResourceDisplayInfo rdi = GameManager.Instance.GetResourceDisplayInfo(resourceID);
        GameObject iconPrefab = rdi.Icon;

        GameObject slot = slots[slotNumber].gameObject;

        GameObject icon = SimplePool.Spawn(iconPrefab, 
                                           slot.transform.position, 
                                           Quaternion.identity);
        icon.transform.SetParent(slot.transform);
        icons.Add(icon);
    }

    void HideResourceIcons()
    {
        foreach(GameObject icon in icons)
        {
            SimplePool.Despawn(icon);
        }
        icons.Clear();
    }

    public void SetStorage(Storage s)
    {
        Storage = s;
        if (Storage != null)
        {
            ShowResourcesInStorage();
        }
    }
}
