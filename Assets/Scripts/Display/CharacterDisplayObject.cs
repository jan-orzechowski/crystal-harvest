using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterDisplayObject : SelectableDisplayObject 
{
    public Character Character;

    public GameObject Model;
    public GameObject HoldingModel;
    GameObject heldResource;

    void Update()
    {
        if (Character == null) { return; }

        if (Character.HasResource)
        {
            Model.SetActive(false);
            HoldingModel.SetActive(true);
            if(heldResource == null)
            {
                ResourceDisplayInfo info = GameManager.Instance.GetResourceDisplayInfo(Character.Resource);
                heldResource = SimplePool.Spawn(
                    info.HeldModel, 
                    this.transform.position, 
                    this.transform.rotation);
                heldResource.transform.SetParent(this.transform);
            }
        }
        else
        {
            Model.SetActive(true);
            HoldingModel.SetActive(false);
            if (heldResource != null)
            {
                SimplePool.Despawn(heldResource);
                heldResource = null;
            }
        }

        if (Character.NextTile != null && Character.CurrentTile != Character.NextTile)
        {
            Vector3 startingPosition = new Vector3(Character.CurrentTile.X + 0.5f, 0, Character.CurrentTile.Y + 0.5f);
            Vector3 goalPosition = new Vector3(Character.NextTile.X + 0.5f, 0, Character.NextTile.Y + 0.5f);

            Vector3 displayPosition = Vector3.Lerp(startingPosition, goalPosition, Character.MovementPercentage);
            transform.SetPositionAndRotation(displayPosition, Character.CurrentRotation);
        }
        else
        {
            transform.SetPositionAndRotation(
                new Vector3(Character.CurrentTile.X + 0.5f, 0, Character.CurrentTile.Y + 0.5f),
                Character.CurrentRotation);
        }        
    }

    public void AssignCharacter(Character character)
    {
        Character = character;
        ModelObject = character;
    }
}
