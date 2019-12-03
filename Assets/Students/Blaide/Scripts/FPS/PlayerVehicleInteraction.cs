using System;
using System.Collections;
using System.Collections.Generic;
using Students.Blaide;
using UnityEngine;

public class PlayerVehicleInteraction : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public GameObject playerCharacterGameObjectObject;
    public Possessable currentPossessed;
    public Possessable playerCharacterPossessable;
    public float maxDistance;
    public float playerClearance = 1f;
    public LayerMask layerMask;
    public Controller controller;

    public event Action<PlayerInfo> OnVehicleExited;
    public event Action<PlayerInfo> OnVehicleEntered; 
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerCharacterPossessable == null)
        {
            Debug.Log("playerCharacterPossessable = null");
        }

    }
    
    public void OnEnterExitButton()
    {
        if (currentPossessed != playerCharacterPossessable)
        {
            ExitVehicle();
        }
        else
        {
            if(LookingAtPossessable()!= null)
                EnterVehicle(LookingAtPossessable());
        }

    }

    private Possessable LookingAtPossessable()
    {
        RaycastHit hit;
        Possessable hitPossessable = null;
       ;
        Vector3 rayDirection = playerCharacterPossessable.virtualCamera.gameObject.transform.TransformDirection(Vector3.forward); //transform.TransformDirection(Vector3.forward)
        Vector3 rayOrigin = playerCharacterPossessable.virtualCamera.gameObject.transform.position + (rayDirection*playerClearance);
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance,
            layerMask))
        {
            if (hit.collider.GetComponent<Possessable>() != null)
            {
                
                hitPossessable = hit.collider.GetComponent<Possessable>();
            }
            else if (hit.collider.transform.root.GetComponent<Possessable>())
            {
                hitPossessable = hit.collider.transform.root.GetComponent<Possessable>();
            }
        }
        return hitPossessable;
    }

    private void EnterVehicle(Possessable p)
    {
        if (p != null)
        {
            Debug.Log("hit a possessable: " + p.gameObject.name);
            currentPossessed.Deactivate();
            currentPossessed = p;
            controller.possessable = p;
            p.virtualCamera.gameObject.layer = playerCharacterPossessable.virtualCamera.gameObject.layer;
            p.virtualCamera.enabled = true;
            playerCharacterGameObjectObject.SetActive(false);
            p.Activate(controller);
            OnVehicleEntered?.Invoke(playerInfo);
            
        }
        else
        {
            Debug.Log("hit nothing");
        }
        
    }

    public void ExitVehicle()
    {
        Vector3 velocity = currentPossessed.GetComponent<Rigidbody>().velocity;
        playerCharacterGameObjectObject.transform.position = currentPossessed.exitPosition.position;
        playerCharacterGameObjectObject.transform.rotation = currentPossessed.exitPosition.rotation;
        currentPossessed.virtualCamera.enabled = false;
        playerCharacterGameObjectObject.SetActive(true);
        currentPossessed.Deactivate();
        currentPossessed = playerCharacterPossessable;
        currentPossessed.Activate(controller);
        currentPossessed.GetComponent<Rigidbody>().velocity = velocity;
        controller.possessable = playerCharacterPossessable;
        OnVehicleExited?.Invoke(playerInfo);
    }


    private void OnDrawGizmos()
    {
        Vector3 rayDirection = playerCharacterPossessable.virtualCamera.gameObject.transform.TransformDirection(Vector3.forward); //transform.TransformDirection(Vector3.forward)
        Vector3 rayOrigin = playerCharacterPossessable.virtualCamera.gameObject.transform.position + (rayDirection*playerClearance);

        Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * maxDistance);
    }
}
