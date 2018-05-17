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

    public Transform IconsParent;
        
    void Awake() 
    {
        ResourceIconSlot[] slotsArray = this.transform.GetComponentsInChildren<ResourceIconSlot>();

        slots = new List<ResourceIconSlot>(slotsArray);
        slots.OrderBy(slot => slot.Order);
        
        icons = new List<GameObject>();
    }

    public void ShowResourceIcon(int resourceID, int slotNumber)
    {
        if (slotNumber >= slots.Count)
        {
            return;
        }

        ResourceDisplayInfo rdi = GameManager.Instance.GetResourceDisplayInfo(resourceID);
        GameObject iconPrefab = rdi.Icon;

        GameObject slot = slots[slotNumber].gameObject;

        GameObject icon = SimplePool.Spawn(iconPrefab, 
                                           slot.transform.position, 
                                           Quaternion.identity);

        if (icon.transform.parent != IconsParent) icon.transform.SetParent(IconsParent);

        icons.Add(icon);
    }

    public void HideResourceIcons()
    {
        foreach (GameObject icon in icons)
        {
            SimplePool.Despawn(icon);
        }
        icons.Clear();
    }
}
