using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterDisplayObject : SelectableDisplayObject 
{
    Character character;

    public GameObject CharacterModel;
    public GameObject CharacterHoldingModel;
    GameObject heldResource;

    void Update()
    {
        if (character == null) { return; }

        if (character.HasResource)
        {
            if (CharacterModel.activeSelf) CharacterModel.SetActive(false);
            if (CharacterHoldingModel.activeSelf == false) CharacterHoldingModel.SetActive(true);

            if(heldResource == null)
            {
                ResourceDisplayInfo info = GameManager.Instance.GetResourceDisplayInfo(character.Resource);
                heldResource = SimplePool.Spawn(
                    info.HeldModel, 
                    this.transform.position, 
                    this.transform.rotation);
                heldResource.transform.SetParent(this.transform);
            }
        }
        else
        {
            if (CharacterModel.activeSelf) CharacterModel.SetActive(true);
            if (CharacterHoldingModel.activeSelf == false) CharacterHoldingModel.SetActive(false);

            if (heldResource != null)
            {
                SimplePool.Despawn(heldResource);
                heldResource = null;
            }
        }

        if (character.NextTile != null && character.CurrentTile != character.NextTile)
        {
            Vector3 startingPosition = new Vector3(character.CurrentTile.X + 0.5f, 0, character.CurrentTile.Y + 0.5f);
            Vector3 goalPosition = new Vector3(character.NextTile.X + 0.5f, 0, character.NextTile.Y + 0.5f);

            Vector3 displayPosition = Vector3.Lerp(startingPosition, goalPosition, character.MovementPercentage);
            transform.SetPositionAndRotation(displayPosition, character.CurrentRotation);
        }
        else
        {
            transform.SetPositionAndRotation(
                new Vector3(character.CurrentTile.X + 0.5f, 0, character.CurrentTile.Y + 0.5f),
                character.CurrentRotation);
        }        
    }

    public void AssignCharacter(Character character)
    {
        this.character = character;
        ModelObject = character;
    }
}
