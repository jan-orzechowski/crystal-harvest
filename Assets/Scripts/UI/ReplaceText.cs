using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ReplaceText : MonoBehaviour 
{
    Text text;
    public string textKey;

    public void UpdateText () 
    {
        text = GetComponent<Text>();

        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }

        if (text == null)
        {
            return;
        }
        else
        {
            if (GameManager.Instance != null)
            {
                text.text = GameManager.Instance.TextManager.GetText(textKey);
            }
            else if (TitleSceneManager.Instance != null)
            {
                text.text = TitleSceneManager.Instance.TextManager.GetText(textKey);
            }            
        }
    }   
}
