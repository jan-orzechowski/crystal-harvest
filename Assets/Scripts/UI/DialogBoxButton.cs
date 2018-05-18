using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogBoxButton : MonoBehaviour 
{
    public DialogBox DialogBox;
    public Text Text;
    public Button Button;

    public void SetAction(Action action)
    {
        Button.onClick.RemoveAllListeners();
        if (action != null)
        {
            Button.onClick.AddListener(() => { action(); });
        }
        Button.onClick.AddListener(() => { DialogBox.ButtonClicked(); });

        Button.onClick.AddListener(() => { GameManager.Instance.SoundManager.PlayButtonSound(); });
    }

    public void SetText(string stringKey)
    {
        Text.text = GameManager.Instance.GetText(stringKey);
    }
}
