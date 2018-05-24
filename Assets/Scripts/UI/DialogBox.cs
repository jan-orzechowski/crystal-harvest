using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogBox : MonoBehaviour 
{
    public SoundManager SoundManager;

    public GameObject Window;    

    public Text DialogText;

    TextManager textManager;

    public GameObject TwoOptionsObject;
    public DialogBoxButton TwoOptionsButtonA;
    public DialogBoxButton TwoOptionsButtonB;

    public GameObject OneOptionObject;
    public DialogBoxButton OneOptionButton;

    public GameObject Blocker;

    void Start () 
    {
        if (GameManager.Instance != null) textManager = GameManager.Instance.TextManager;
        else if (TitleSceneManager.Instance != null) textManager = TitleSceneManager.Instance.TextManager;

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
        SetDialogText(textManager.GetText(descriptionTextKey));

        Window.SetActive(true);
        Blocker.SetActive(true);

        OneOptionObject.SetActive(false);
        TwoOptionsObject.SetActive(true);

        TwoOptionsButtonA.SetText(textManager.GetText(optionATextKey));
        TwoOptionsButtonA.SetAction(optionAAction);

        TwoOptionsButtonB.SetText(textManager.GetText(optionBTextKey));
        TwoOptionsButtonB.SetAction(optionBAction);
    }

    public void ShowDialogBox(string descriptionTextKey,
                              string optionTextKey, Action optionAction,
                              bool hideAfterClick = true)
    {
        SetDialogText(GameManager.Instance.GetText(descriptionTextKey));

        Window.SetActive(true);
        Blocker.SetActive(true);

        TwoOptionsObject.SetActive(false);
        OneOptionObject.SetActive(true);

        OneOptionButton.SetText(textManager.GetText(optionTextKey));
        OneOptionButton.SetAction(optionAction, hideAfterClick);
    }

    void HideDialogBox()
    {
        Window.SetActive(false);
        TwoOptionsObject.SetActive(false);
        OneOptionObject.SetActive(false);
        Blocker.SetActive(false);
    }

    public void ButtonClicked()
    {
        HideDialogBox();
    }
}
