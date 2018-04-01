using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HoverBuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string PrototypeName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BuildingPrototype prototype = GameManager.Instance.World.GetBuildingPrototype(PrototypeName);
        if (prototype != null)
        {
            GameManager.Instance.Tooltip.SetText(prototype.Description, 0.1f);
        }        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.Tooltip.Hide();
    }
}
