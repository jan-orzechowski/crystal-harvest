using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogBox : MonoBehaviour 
{
    public GameObject Window;

    public Text DialogText;

    public GameObject TwoOptionsObject;
    public DialogBoxButton TwoOptionsButtonA;
    public DialogBoxButton TwoOptionsButtonB;

    public GameObject OneOptionObject;
    public DialogBoxButton OneOptionButton;

    void Start () 
    {
        HideDialogBox();
    }
   
    void SetDialogText(string newText)
    {
        // Nie użyto TextGeneratora ze względu na błąd: 
        // przy zaznaczonej opcji HorizontalWrap zwraca błędną wysokość i liczbę linii

        DialogText.text = newText;

        int approxMaxCharactersInLine = 25;
        float lineHeight = 25f;
        float estimatedLineCount = Math.Max(1, Mathf.CeilToInt(DialogText.text.Length / approxMaxCharactersInLine));
        float height = estimatedLineCount * lineHeight;

        DialogText.rectTransform.sizeDelta = new Vector2(DialogText.rectTransform.rect.width, height);        
    }

    public void ShowDialogBox(string descriptionTextKey, 
                              string optionATextKey, Action optionAAction, 
                              string optionBTextKey, Action optionBAction)
    {
        SetDialogText(GameManager.Instance.GetText(descriptionTextKey));

        Window.SetActive(true);

        OneOptionObject.SetActive(false);
        TwoOptionsObject.SetActive(true);

        TwoOptionsButtonA.SetText(optionATextKey);
        TwoOptionsButtonA.SetAction(optionAAction);

        TwoOptionsButtonB.SetText(optionBTextKey);
        TwoOptionsButtonB.SetAction(optionBAction);
    }

    public void ShowDialogBox(string descriptionTextKey,
                              string optionTextKey, Action optionAction)
    {
        SetDialogText(GameManager.Instance.GetText(descriptionTextKey));

        Window.SetActive(true);

        TwoOptionsObject.SetActive(false);
        OneOptionObject.SetActive(true);

        OneOptionButton.SetText(optionTextKey);
        OneOptionButton.SetAction(optionAction);
    }

    void HideDialogBox()
    {
        Window.SetActive(false);
        TwoOptionsObject.SetActive(false);
        OneOptionObject.SetActive(false);
    }

    public void ButtonClicked()
    {
        HideDialogBox();
    }
}
