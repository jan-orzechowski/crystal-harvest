using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBar : MonoBehaviour 
{
    public Image FillImage;
    public Text Text;

    public void SetFillPercentage(float percentage)
    {
        FillImage.fillAmount = percentage;
        Text.text = Mathf.FloorToInt(percentage * 100) + "%";
    }

    public void SetFillPercentageWithoutText(float percentage)
    {
        FillImage.fillAmount = percentage;
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

}
