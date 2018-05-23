using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PauseSign : MonoBehaviour 
{
    public GameObject Sign;

    bool active;

    void Update () 
    {
        if (GameManager.Instance.World.Paused && active == false)
        {
            Sign.SetActive(true);
            active = true;
        }
        else if (GameManager.Instance.World.Paused == false && active)
        {
            Sign.SetActive(false);
            active = false;
        }
    }
}
