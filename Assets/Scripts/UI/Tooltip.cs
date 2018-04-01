using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Tooltip : MonoBehaviour 
{
    public RectTransform Display;
    public Text Text;
    public float Timer;

    float timeToShow;
    string textToSet;

    TextGenerator textGen;

    void Start()
    {
        textGen = new TextGenerator();
    }

    void Update()
    {
        if (textToSet != null)
        {
            timeToShow -= Time.deltaTime;
            if (timeToShow <= 0f)
            {
                Text.text = textToSet;               
                TextGenerationSettings generationSettings = Text.GetGenerationSettings(Text.rectTransform.rect.size);
                float width = textGen.GetPreferredWidth(textToSet, generationSettings);
                Display.sizeDelta = new Vector2(width + 20, Display.rect.height);
                Display.gameObject.SetActive(true);
                textToSet = null;
            }
        }

        if (Display.gameObject.activeSelf)
        {
            if (Input.mousePosition.x + Display.rect.width > Screen.width)
            {
                Display.transform.SetPositionAndRotation(
                    new Vector3(Input.mousePosition.x - Display.rect.width,
                                Input.mousePosition.y, Input.mousePosition.z),
                    Quaternion.identity);
            }
            else
            {
                Display.transform.SetPositionAndRotation(Input.mousePosition, Quaternion.identity);
            }
        }
    }
    
    public void SetText(string newText, float timeToShow)
    {
        if (textToSet == newText) return;

        timeToShow = Timer;
        textToSet = newText;
    }

    public void SetText(string newText)
    {
        SetText(newText, Timer);
    }

    public void Hide()
    {
        Display.gameObject.SetActive(false);
        textToSet = null;
    }
}
