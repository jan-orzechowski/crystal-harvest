using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogBox : MonoBehaviour 
{
    public int maxButtonsNumber = 10;

    Action[] Actions;

    public Text DialogText;
    public RectTransform DialogTextRect;

    public Text ButtonAText;
    public Text ButtonBText;

    TextGenerator textGen;

    float textFieldWidth;

    void Start () 
    {
        textGen = new TextGenerator();
    }
   
    public void SetDialogText(string newText)
    {
        DialogText.text = newText;
        TextGenerationSettings generationSettings = DialogText.GetGenerationSettings(DialogText.rectTransform.rect.size);
        float height = textGen.GetPreferredHeight(newText, generationSettings);
        DialogTextRect.sizeDelta = new Vector2(DialogTextRect.rect.width, height);


    }

    public void ChooseA()
    {

    }

    public void ChooseB()
    {

    }

    public void SetButtonA(Action action, string text)
    {
        //ButtonAAction = action;
        ButtonAText.text = text;
    }

    public void SetButtonB(Action action, string text)
    {
        //ButtonBAction = action;
        ButtonBText.text = text;
    }
}
