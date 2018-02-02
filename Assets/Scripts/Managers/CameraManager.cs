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
        mainCamera.position = new Vector3(0, startingHeight, -(startingHeight *Mathf.Tan(cameraAngle * Mathf.Deg2Rad)));

        cameraRig = mainCamera.parent;

        // Wyśrodkowanie kamery
        //cameraRig.position = new Vector3(
        //    GameManager.Instance.World.XSize / 2, 0f, GameManager.Instance.World.YSize / 2);
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

        mainCamera.RotateAround(cameraRig.position, mainCamera.up, rotation);

        Quaternion lookRotation = Quaternion.LookRotation(-mainCamera.localPosition);
        mainCamera.rotation = lookRotation;
    }
}
