using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectableDisplayObject : MonoBehaviour 
{
    public BoxCollider Collider;
    public ISelectable ModelObject;
   
    void Start () 
    {
        if(Collider == null)
        {
            Collider = GetComponentInChildren<BoxCollider>();
        }
    }
        
    public virtual void AssignModelObject(ISelectable modelObject)
    {
        ModelObject = modelObject;
    }
}
