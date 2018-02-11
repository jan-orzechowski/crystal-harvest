using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectableDisplayObject : MonoBehaviour 
{
    public Collider Collider;
    public ISelectable ModelObject;
   
    void Start () 
    {
        if(Collider == null)
        {
            Collider = GetComponentInChildren<Collider>();
        }
    }
        
    public virtual void AssignModelObject(ISelectable modelObject)
    {
        ModelObject = modelObject;
    }
}
