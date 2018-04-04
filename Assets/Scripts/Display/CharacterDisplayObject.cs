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

    public bool Hidden { get; private set; }
    float hideTimer;
    bool gameObjectsDeactivated;

    void Awake()
    {
        CharacterModel.SetActive(false);
        CharacterHoldingModel.SetActive(false);
    }

    void Update()
    {
        if (character == null) { return; }

        if (Hidden)
        {
            if (gameObjectsDeactivated == false)
            {
                CharacterModel.SetActive(false);
                CharacterHoldingModel.SetActive(false);
                Collider.gameObject.SetActive(false);

                gameObjectsDeactivated = true;
            }
            
            
            hideTimer -= Time.deltaTime;

            if (hideTimer > 0f)
            {
                return;
            }
            else
            {
                Collider.gameObject.SetActive(true);
                Hidden = false;
                gameObjectsDeactivated = false;
            }
        }

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
            if (CharacterModel.activeSelf == false) CharacterModel.SetActive(true);
            if (CharacterHoldingModel.activeSelf) CharacterHoldingModel.SetActive(false);

            if (heldResource != null)
            {
                SimplePool.Despawn(heldResource);
                heldResource = null;
            }
        }

        if (character.NextTile != null && character.CurrentTile != character.NextTile)
        {
            Vector3 startingPosition = new Vector3(character.CurrentTile.X + 0.5f, 
                                                   character.CurrentTile.Height, 
                                                   character.CurrentTile.Y + 0.5f);

            Vector3 goalPosition = new Vector3(character.NextTile.X + 0.5f,
                                               character.NextTile.Height,
                                               character.NextTile.Y + 0.5f);

            Vector3 displayPosition = Vector3.Lerp(startingPosition, 
                                                   goalPosition, 
                                                   character.MovementPercentage);

            transform.SetPositionAndRotation(displayPosition, character.CurrentRotation);
        }
        else
        {
            transform.SetPositionAndRotation(
                new Vector3(character.CurrentTile.X + 0.5f, 
                            character.CurrentTile.Height, 
                            character.CurrentTile.Y + 0.5f),
                character.CurrentRotation);
        }        
    }

    public void AssignCharacter(Character character)
    {
        this.character = character;
        ModelObject = character;
    }

    public void CharacterUsesModule(IBuildingModule module)
    {
        if (module.HidesCharacter)
        {
            hideTimer = 0.1f;
            Hidden = true;
        }
    }
}
