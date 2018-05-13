using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildingModulePanel : MonoBehaviour 
{
    IBuildingModule Module;

    public RectTransform DeconstructButton;
    public RectTransform CancelDeconstructionButton;
    public RectTransform HaltButton;
    public RectTransform StartButton;

    public float FirstButtonOffset = 0f;
    public float SecondButtonOffset = 50f;
    public float ButtonXPosition = -350;

    bool changed;

    bool canBeHalted;
    bool canBeDeconstructed;

    float deconstructButtonOffset;
    float haltButtonOffset;

    void Update()
    {
        if (changed == false) return;

        if (canBeHalted && canBeDeconstructed)
        {
            deconstructButtonOffset = FirstButtonOffset;
            haltButtonOffset = SecondButtonOffset;
        }
        else if (canBeHalted) haltButtonOffset = FirstButtonOffset;
        else if (canBeDeconstructed) deconstructButtonOffset = FirstButtonOffset;

        HaltButton.anchoredPosition = new Vector2(ButtonXPosition, haltButtonOffset);
        StartButton.anchoredPosition = new Vector2(ButtonXPosition, haltButtonOffset);
        DeconstructButton.anchoredPosition = new Vector2(ButtonXPosition, deconstructButtonOffset);
        CancelDeconstructionButton.anchoredPosition = new Vector2(ButtonXPosition, deconstructButtonOffset);

        if (canBeHalted)
        {
            if (Module.Halted)
            {
                HaltButton.gameObject.SetActive(false);
                StartButton.gameObject.SetActive(true);
            }
            else
            {
                HaltButton.gameObject.SetActive(true);
                StartButton.gameObject.SetActive(false);
            }
        }
        else
        {
            HaltButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
        }

        if (canBeDeconstructed)
        {
            if (Module.IsPreparingForDeconstruction())
            {
                DeconstructButton.gameObject.SetActive(false);
                CancelDeconstructionButton.gameObject.SetActive(true);

                HaltButton.gameObject.SetActive(false);
                StartButton.gameObject.SetActive(false);
            }
            else
            {
                DeconstructButton.gameObject.SetActive(true);
                CancelDeconstructionButton.gameObject.SetActive(false);
            }
        }
        else
        {
            DeconstructButton.gameObject.SetActive(false);
            CancelDeconstructionButton.gameObject.SetActive(false);
        }    
    }

    public void DeconstructButtonAction()
    {
        if (Module == null || Module.Building == null) return;

        Action action = () => 
        {
            if (Module == null || Module.Building == null) return;
            GameManager.Instance.World.MarkBuildingForDenconstruction(Module.Building);
            changed = true;
        };

        GameManager.Instance.DialogBox.ShowDialogBox(
            "s_deconstruction_prompt",
            "s_yes", action,
            "s_no", null);        
    }

    public void CancelDeconstructionButtonAction()
    {
        if (Module == null) return;
        GameManager.Instance.World.CancelBuildingDeconstruction(Module.Building);
        changed = true;
    }

    public void HaltButtonAction()
    {
        if (Module == null) return;
        Module.SetHalt(true);
        changed = true;       
    }

    public void StartButtonAction()
    {
        if (Module == null) return;
        Module.SetHalt(false);
        changed = true;
    }

    public void SetModule(IBuildingModule module)
    {        
        Module = module;

        canBeHalted = false;
        canBeDeconstructed = false;

        if (Module != null)
        {
            if (Module is ConstructionSite)
            {
                canBeHalted = true;
                if (((ConstructionSite)Module).ConstructionMode)
                {
                    canBeDeconstructed = true;
                }
            }
            else if (Module is Storage)
            {
                canBeDeconstructed = true;
            }
            else if (Module is Factory)
            {
                canBeHalted = true;
                canBeDeconstructed = true;
            }
            else if (Module is Service)
            {
                canBeDeconstructed = true;
            }

            if (module.Building.Prototype.CanBeDeconstructed == false)
            {
                canBeDeconstructed = false;
            }
        }

        changed = true;
        Update();
    }
}
