using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TitleSceneManager : MonoBehaviour 
{
    public static TitleSceneManager Instance;

    public TextManager TextManager;

    public DialogBox DialogBox;

    public Text VersionText;

    public GameObject MenuPanel;

    public GameObject CreditsPanel;
    bool creditsPanelActive;

    public GameObject SoundOnButton;
    public GameObject SoundOffButton;

    public GameObject QuitButton;

    void Awake () 
    {
#if UNITY_WEBGL
        QuitButton.gameObject.SetActive(false);
#endif
        Instance = this;        
        VersionText.text = StaticData.Version;
        TextManager = new TextManager();

        CreditsPanel.SetActive(true);
        ReplaceText[] textFields = CreditsPanel.GetComponentsInChildren<ReplaceText>();
        foreach(ReplaceText field in textFields)
        {
            field.UpdateText();
        }
        CreditsPanel.SetActive(false);

        MenuPanel.SetActive(true);
        textFields = MenuPanel.GetComponentsInChildren<ReplaceText>();
        foreach (ReplaceText field in textFields)
        {
            field.UpdateText();
        }

        SoundButtonUpdate();
    }

    private void Update()
    {
        if (creditsPanelActive)
        {
            if (Input.anyKeyDown)
            {
                MenuPanel.SetActive(true);
                CreditsPanel.SetActive(false);
                creditsPanelActive = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitButtonAction();
            }
        }        
    }

    public void NewGameButtonAction()
    {        
        SceneManager.LoadScene("main_scene");
    }

    public void CreditsButtonAction()
    {
        MenuPanel.SetActive(false);
        CreditsPanel.SetActive(true);
        creditsPanelActive = true;
    }

    public void QuitButtonAction()
    {
        Action action = () =>
        {
            Application.Quit();
        };

        DialogBox.ShowDialogBox(
            "s_quit_prompt",
            "s_yes", action,
            "s_no", null);        
    }

    public void SoundButtonUpdate()
    {
        SoundOnButton.SetActive(SoundManager.Muted == true);
        SoundOffButton.SetActive(SoundManager.Muted == false);
    }
}
