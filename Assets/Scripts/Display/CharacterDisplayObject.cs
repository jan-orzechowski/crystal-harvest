using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterDisplayObject : SelectableDisplayObject 
{
    Character character;

    public GameObject CharacterModel;
    public GameObject CharacterHoldingModel;
    public Material ClippableMaterial;
    GameObject heldResource;

    public bool Hidden { get; private set; }
    float hideTimer;
    bool gameObjectsDeactivated;

    public bool DeathAnimationStarted { get; private set; }

    float deathAnimationPercentage;
    float deathAnimationPercentagePerSecond = 0.3f;

    public bool DeathAnimationPlayed { get; protected set; }

    void Awake()
    {
        CharacterModel.SetActive(false);
        CharacterHoldingModel.SetActive(false);
        CharacterModel.transform.localPosition = Vector3.zero;
        CharacterModel.transform.localRotation = Quaternion.identity;
        CharacterHoldingModel.transform.localPosition = Vector3.zero;
        CharacterHoldingModel.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (character == null) { return; }

        if (DeathAnimationStarted && DeathAnimationPlayed == false)
        {
            deathAnimationPercentage += deathAnimationPercentagePerSecond * Time.deltaTime;

            if (deathAnimationPercentage >= 1f)
            {
                DeathAnimationPlayed = true;
                return;
            }

            GameObject currentModel = heldResource ? CharacterHoldingModel : CharacterModel;

            float xAxisRotation;
            float yPos;

            if (deathAnimationPercentage <= 0.5f)
            {
                xAxisRotation = Mathf.Lerp(0f, -90f, deathAnimationPercentage * 2f);
                yPos = 0;
            }
            else
            {
                xAxisRotation = -90f;
                yPos = Mathf.Lerp(0f, -0.2f, (deathAnimationPercentage - 0.5f) * 2f);
            }

            currentModel.transform.localPosition =
                new Vector3(currentModel.transform.localPosition.x,
                            yPos,
                            currentModel.transform.localPosition.z);

            currentModel.transform.localRotation = 
                Quaternion.Euler(xAxisRotation,
                                 currentModel.transform.localRotation.eulerAngles.y,
                                 currentModel.transform.localRotation.eulerAngles.z);

            currentModel.GetComponent<Renderer>().material = ClippableMaterial;

            return;
        }

        if (Hidden)
        {
            if (gameObjectsDeactivated == false)
            {
                CharacterModel.SetActive(false);
                CharacterHoldingModel.SetActive(false);
                Collider.gameObject.SetActive(false);

                gameObjectsDeactivated = true;
            }
            
            if (GameManager.Instance.World.Paused == false)
            {
                hideTimer -= Time.deltaTime;
            }

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

            if (heldResource == null)
            {
                ResourceDisplayInfo info = GameManager.Instance.GetResourceDisplayInfo(character.Resource);
                heldResource = SimplePool.Spawn(
                    info.HeldModel, 
                    this.transform.position, 
                    this.transform.rotation);
                heldResource.transform.SetParent(CharacterHoldingModel.transform);
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
        if (module == null)
        {
            Hidden = false;
            gameObjectsDeactivated = false;
            Collider.gameObject.SetActive(true);

            return;
        }

        if (module.HidesCharacter)
        {
            hideTimer = 0.1f;
            Hidden = true;
        }
    }

    public void PlayDeathAnimation()
    {
        if (DeathAnimationStarted == false)
        {
            DeathAnimationStarted = true;
            deathAnimationPercentage = 0f;
        }        
    }

}
