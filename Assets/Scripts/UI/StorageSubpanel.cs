using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class StorageSubpanel : MonoBehaviour 
{
    List<ResourceIconSlot> slots;
    
    List<GameObject> icons;

    public Text TextSubpanel;
    
    void Awake () 
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();
        if (slotsArray == null || slotsArray.Length == 0)
        {
            Debug.Log("Panel nie ma żadnych slotów na zasoby!");
        }
        slots = new List<ResourceIconSlot>(slotsArray);
        slots.OrderBy(slot => slot.Order);
        
        icons = new List<GameObject>();
    }

    public void ShowResourceIcon(int resourceID, int slotNumber)
    {
        if (slotNumber >= slots.Count)
        {
            //Debug.Log("Chcemy pokazać więcej zasobów niż panel na to pozwala!");
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

    public void HideResourceIcons()
    {
        foreach(GameObject icon in icons)
        {
            SimplePool.Despawn(icon);
        }
        icons.Clear();
    }
}
