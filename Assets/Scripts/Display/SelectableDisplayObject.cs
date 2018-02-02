using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectableDisplayObject : MonoBehaviour 
{
    public Collider Collider;
    public ISelectable ModelObject;

    public Character debugCharacter;

    void Start () 
    {
        if(Collider == null)
        {
            Collider = GetComponentInChildren<Collider>();
        }
    }
    
    void Update () 
    {
        if (debugCharacter != null)
        {
            if (debugCharacter.NextTile != null && debugCharacter.CurrentTile != debugCharacter.NextTile)
            {
                Vector3 startingPosition = new Vector3(debugCharacter.CurrentTile.X, 0, debugCharacter.CurrentTile.Y);
                Vector3 goalPosition = new Vector3(debugCharacter.NextTile.X, 0, debugCharacter.NextTile.Y);
                Vector3 displayPosition = Vector3.Lerp(startingPosition, goalPosition, debugCharacter.MovementPercentage);
                transform.SetPositionAndRotation(displayPosition, Quaternion.identity);
            }
            else
            {
                transform.SetPositionAndRotation(
                    new Vector3(debugCharacter.CurrentTile.X, 0, debugCharacter.CurrentTile.Y), 
                    Quaternion.identity);
            }
        }
    }

    public void AssignModelObject(ISelectable modelObject)
    {
        ModelObject = modelObject;
        if (modelObject is Character)
        {
            debugCharacter = (Character)modelObject;
        }
    }
}
