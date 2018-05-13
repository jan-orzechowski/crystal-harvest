using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour 
{
    Transform mainCamera;
    Transform cameraRig;

    float cameraAngle = 30f;
    float startingHeight = 8f;

    float minDistance = 4;
    float maxDistance = 15;

    float moveMultiplier = 0.25f;
    float zoomMultiplier = 1f;
    float rotationSpeed = 1f;
   
    void Start()
    {
        mainCamera = Camera.main.transform;        
        mainCamera.position = new Vector3(0, startingHeight, -(startingHeight * Mathf.Tan(cameraAngle * Mathf.Deg2Rad)));

        Quaternion lookRotation = Quaternion.LookRotation(-mainCamera.localPosition);
        mainCamera.rotation = lookRotation;

        cameraRig = mainCamera.parent;

        float startingCameraXPosition = GameManager.Instance.World.StartingAreaX + 2.5f;
        float startingCameraYPosition = GameManager.Instance.World.StartingAreaY + 3f;

        cameraRig.SetPositionAndRotation(
            new Vector3(startingCameraXPosition, 0f, startingCameraYPosition), 
            Quaternion.identity);

        mainCamera.RotateAround(cameraRig.position, cameraRig.up, 45f);
    }

    void Update()
    {
        MoveCamera();
        ZoomCamera();
        RotateCamera();
    }

    void MoveCamera()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 translation = input * moveMultiplier;
        translation = Quaternion.Euler(0, mainCamera.rotation.eulerAngles.y, 0) * translation;

        cameraRig.position = cameraRig.position + translation;
    }

    void ZoomCamera()
    {
        float input = -Input.GetAxis("Mouse ScrollWheel");
      
        Vector3 translation = mainCamera.localPosition * zoomMultiplier * input;
        Vector3 newPosition = mainCamera.localPosition + translation;
        if (newPosition.normalized.y < 0.01f)
        {
            return;
        }

        newPosition = newPosition.normalized * Mathf.Clamp(newPosition.magnitude, minDistance, maxDistance);
        mainCamera.localPosition = newPosition;
    }

    void RotateCamera()
    {
        float rotation = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotation += rotationSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotation -= rotationSpeed;
        }

        mainCamera.RotateAround(cameraRig.position, cameraRig.up, rotation);        
    }
}
