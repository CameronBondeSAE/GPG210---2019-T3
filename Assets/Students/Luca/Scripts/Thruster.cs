using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Thruster : MonoBehaviour
{
    public Vehicle master;
    
    public float zeroForceHeight = 3;
    
    public float currentDistanceToGround = 0;

    public AnimationCurve forceHeightCurve;
    
    public Vector3 maxForce;

    public float tiltSpeed = 30; // Angle per second
    public float maxTiltAngle = 15;
    public bool inputTiltRight = false;
    public bool inputTiltLeft = false;
    public bool inputTiltForward = false;
    public bool inputTiltBackward = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTiltInput();
        
        // HACK  Inperformant
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.up*-1 * currentDistanceToGround, Color.blue);
        if (Physics.Raycast(transform.position, transform.up*-1, out hit, zeroForceHeight)) // TODO do raycast from bottom/exhaust
        {
            currentDistanceToGround = hit.distance;
        }
        else
        {
            currentDistanceToGround = zeroForceHeight;
        }
        
        AddForce();
    }

    public void AddForce()
    {
        float curveValue = Mathf.Clamp(currentDistanceToGround, 0, zeroForceHeight) / zeroForceHeight;
        Vector3 finalForce = transform.TransformDirection(forceHeightCurve.Evaluate(curveValue) * maxForce);
        
        master.AddForce(transform.localPosition, finalForce);
    }

    void HandleTiltInput()
    {
        float tiltAmount = tiltSpeed * Time.deltaTime;
        Vector3 tiltChange = Vector3.zero;

        float localZRot = transform.localRotation.eulerAngles.z;
        // Handle Tilt Right
        if (inputTiltRight && localZRot+tiltAmount <= maxTiltAngle)
        {
            tiltChange.z += tiltAmount;
        }
        // Handle Tilt Left
        if (inputTiltLeft && ((localZRot - tiltAmount + 360) % 360) >= 360-maxTiltAngle && (localZRot-tiltAmount <= 360 || localZRot-tiltAmount <= 0))
        {
            tiltChange.z -= tiltAmount;
        }
        
        // RESET Left/Right
        if (!inputTiltRight && !inputTiltLeft && !Mathf.Approximately(localZRot,0))
        {
            /*int sign = localZRot <= maxTiltAngle ? -1 : 1;
            float finalTiltAmount = sign * Mathf.Abs(localZRot) - tiltAmount < 0? Mathf.Abs(localZRot) : tiltAmount;
            tiltChange.z += finalTiltAmount; //localZRot <= maxTiltAngle ? -tiltAmount : tiltAmount; // USE MOVETOWARDS OR SO.... */
            tiltChange.z += localZRot <= maxTiltAngle ? -tiltAmount : tiltAmount; // USE MOVETOWARDS OR SO.... 
        }
        
        float localXRot = transform.localRotation.eulerAngles.x;
        // Handle Tilt Forward
        if (inputTiltForward && localXRot+tiltAmount <= maxTiltAngle)
        {
            tiltChange.x += tiltAmount;
        }
        // Handle Tilt Backward
        if (inputTiltBackward && ((localXRot - tiltAmount + 360) % 360) >= 360-maxTiltAngle && (localXRot-tiltAmount <= 360 || localXRot-tiltAmount <= 0))
        {
            tiltChange.x -= tiltAmount;
        }
        
        // RESET Forward/Backward
        if (!inputTiltForward && !inputTiltBackward && !Mathf.Approximately(localXRot,0))
        {
            /*int sign = localXRot <= maxTiltAngle ? -1 : 1;
            float finalTiltAmount = sign * Mathf.Abs(localXRot) - tiltAmount < 0? Mathf.Abs(localXRot) : tiltAmount;
            tiltChange.x += finalTiltAmount;//localXRot <= maxTiltAngle ? -tiltAmount : tiltAmount; // USE MOVETOWARDS OR SO.... */
            tiltChange.x += localXRot <= maxTiltAngle ? -tiltAmount : tiltAmount; // USE MOVETOWARDS OR SO.... 
        }
        
        Tilt(tiltChange);
    }

    private void Tilt(Vector3 eulerChange)
    {
        Vector3 newRotEuler = transform.localRotation.eulerAngles;
        newRotEuler += eulerChange;
        transform.localRotation = Quaternion.Euler(newRotEuler);
    }

    /*private void TiltRight()
    {
        float tiltAmount = tiltSpeed * Time.deltaTime;
        if (transform.localRotation.eulerAngles.z + tiltAmount < maxTiltAngle)
        {
            Vector3 newRotEuler = transform.localRotation.eulerAngles;
            newRotEuler.z += tiltAmount;
            transform.localRotation = Quaternion.Euler(newRotEuler);
        }
        //transform.Rotate( transform.TransformDirection(Vector3.forward), tiltAmount);
    }*/
    
    
}
