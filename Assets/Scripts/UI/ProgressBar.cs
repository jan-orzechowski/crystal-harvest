using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressBar : MonoBehaviour 
{
    public Image FillImage;

    public void SetFillPercentage(float percentage)
    {
        FillImage.fillAmount = percentage;
    }
}
