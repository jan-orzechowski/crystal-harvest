using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SidePanel : MonoBehaviour 
{
    public SoundManager SoundManager;
    public InputManager InputManager;

    public GameObject BuildPanel;
    public GameObject BuildPanelButton;

    public GameObject StatsPanel;
    public GameObject StatsPanelButton;

    public GameObject SoundOnButton;
    public GameObject SoundOffButton;

    public GameObject InfoButton; 

    public bool PanelVisible { get; protected set; }

    void Start () 
    {
        HidePanels();
        ShowSoundButton(true);
    }
    
    public void ShowBuildPanel()
    {
        PanelVisible = true;
        HideButtons();
        BuildPanel.SetActive(true);
        InputManager.RemoveSelection();
        InputManager.SelectionPanel.HidePanels();
        InputManager.SetBuildMode(false);
    }

    public void ShowStatsPanel()
    {
        PanelVisible = true;
        HideButtons();
        StatsPanel.SetActive(true);
        InputManager.RemoveSelection();
        InputManager.SelectionPanel.HidePanels();
        InputManager.SetBuildMode(false);
    }

    void HideButtons()
    {
        BuildPanelButton.SetActive(false);
        StatsPanelButton.SetActive(false);
        InfoButton.SetActive(false);

        ShowSoundButton(false);
    }

    public void HidePanels()
    {
        if (PanelVisible)
        {
            PanelVisible = false;

            BuildPanel.SetActive(false);
            StatsPanel.SetActive(false);
            BuildPanelButton.SetActive(true);
            StatsPanelButton.SetActive(true);
            InfoButton.SetActive(true);

            ShowSoundButton(true);

            GameManager.Instance.Tooltip.Hide();
        }
    }

    public void SoundOnButtonAction()
    {
        SoundManager.TurnSoundOn();
        ShowSoundButton(true);
    }

    public void SoundOffButtonAction()
    {
        SoundManager.TurnSoundOff();
        ShowSoundButton(true);
    }

    public void InfoButtonAction()
    {
        Action tips3 = () =>
        {
            GameManager.Instance.DialogBox.ShowDialogBox(
                "s_tips_3", 
                "s_tips_end", null,
                hideAfterClick: true);
        };

        Action tips2 = () =>
        {
            GameManager.Instance.DialogBox.ShowDialogBox(
                "s_tips_2",
                "s_tips_next", tips3,
                hideAfterClick: false);
        };

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_tips_1",
            "s_tips_next", tips2,
            hideAfterClick: false);
    }

    void ShowSoundButton(bool show)
    {
        if (show == false)
        {
            SoundOnButton.SetActive(false);
            SoundOffButton.SetActive(false);
        }
        else
        {
            if (SoundManager.Muted)
            {
                SoundOffButton.SetActive(false);
                SoundOnButton.SetActive(true);
            }
            else
            {
                SoundOffButton.SetActive(true);
                SoundOnButton.SetActive(false);
            }
        }
    }
}
