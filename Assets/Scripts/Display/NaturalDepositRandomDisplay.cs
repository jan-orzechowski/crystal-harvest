using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NaturalDepositRandomDisplay : MonoBehaviour
{
    public string Resource;
   
    void Awake()
    {
        if (GameManager.Instance == null) { return; }

        NaturalDepositDisplayInfo[] displays = GameManager.Instance.depositDisplay;

        for (int i = 0; i < displays.Length; i++)
        {
            if (displays[i] != null && displays[i].Resource == Resource)
            {
                int prefabIndex = UnityEngine.Random.Range(0, displays[i].DisplayObjects.Length);
                GameObject prefab = displays[i].DisplayObjects[prefabIndex];
                GameObject display = SimplePool.Spawn(prefab, this.transform.position, Quaternion.identity);
                display.transform.SetParent(this.transform);                                 
                return;
            }
        }  
    }
}
