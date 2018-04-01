using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SidePanel : MonoBehaviour 
{
    public GameObject BuildPanel;
    public GameObject BuildPanelButton;

    public GameObject StatsPanel;
    public GameObject StatsPanelButton;

    public InputManager InputManager;

    public bool PanelVisible { get; protected set; }

    void Start () 
    {
        HidePanels();
    }
    
    public void ShowBuildPanel()
    {
        PanelVisible = true;
        HideButtons();
        BuildPanel.SetActive(true);
        InputManager.SetBuildMode(false);
    }

    public void ShowStatsPanel()
    {
        PanelVisible = true;
        HideButtons();
        StatsPanel.SetActive(true);
        InputManager.SetBuildMode(false);
    }

    void HideButtons()
    {
        BuildPanelButton.SetActive(false);
        StatsPanelButton.SetActive(false);
    }

    public void HidePanels()
    {
        PanelVisible = false;
        BuildPanel.SetActive(false);
        StatsPanel.SetActive(false);
        BuildPanelButton.SetActive(true);
        StatsPanelButton.SetActive(true);
        GameManager.Instance.Tooltip.Hide();
    }
}
