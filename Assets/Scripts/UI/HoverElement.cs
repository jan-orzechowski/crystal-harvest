using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class HoverElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string HoverTextKey;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.Tooltip.SetText(GameManager.Instance.GetText(HoverTextKey));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.Tooltip.Hide();
    }

    void OnDisable()
    {
        GameManager.Instance.Tooltip.Hide();
    }
}
