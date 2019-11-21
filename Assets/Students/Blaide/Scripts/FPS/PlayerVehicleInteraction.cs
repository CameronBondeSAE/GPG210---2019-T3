﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicleInteraction : MonoBehaviour
{
    public GameObject playerCharacterGameObjectObject;
    public Possessable currentPossessed;
    public Possessable playerCharacterPossessable;
    public float maxDistance;
    public LayerMask layerMask;
    public Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnEnterExitButton()
    {
        if (currentPossessed != playerCharacterPossessable)
        {
            ExitVehicle();
        }
        else
        {
            EnterVehicle(lookingAtPossessable());
        }

    }

    private Possessable lookingAtPossessable()
    {
        RaycastHit hit;
        Possessable hitPossessable = null;
        float playerClearance = 1;
        Vector3 rayDirection = playerCharacterPossessable.virtualCamera.gameObject.transform.TransformDirection(Vector3.forward); //transform.TransformDirection(Vector3.forward)
        Vector3 rayOrigin = playerCharacterPossessable.virtualCamera.gameObject.transform.position + (rayDirection*playerClearance);
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance,
            layerMask))
        {
            if (hit.collider.GetComponent<Possessable>() != null)
            {
                hitPossessable = hit.collider.GetComponent<Possessable>();
            }
        }
        return hitPossessable;
    }

    private void EnterVehicle(Possessable p)
    {
        if (p != null)
        {
            Debug.Log("hit a possessable: " + p.gameObject.name);
            currentPossessed = p;
            controller.possessable = p;
            p.virtualCamera.gameObject.layer = playerCharacterPossessable.virtualCamera.gameObject.layer;
            p.virtualCamera.enabled = true;
            playerCharacterGameObjectObject.SetActive(false);
        }
        else
        {
            Debug.Log("hit nothing");
        }
        
    }

    private void ExitVehicle()
    {
        playerCharacterGameObjectObject.transform.position = currentPossessed.exitPosition.position;
        playerCharacterGameObjectObject.transform.rotation = currentPossessed.exitPosition.rotation;
        currentPossessed.virtualCamera.enabled = false;
        playerCharacterGameObjectObject.SetActive(true);
        currentPossessed = playerCharacterPossessable;
        controller.possessable = playerCharacterPossessable;
    }
}