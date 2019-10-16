﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMainSystem : MonoBehaviour
{
    public float baseEngineTorque;
    public AnimationCurve springCurve;
    public List<WheelScript> wheels;
    public float forceMultiplier;
    public float maxDistance;
    public float accelerator;
    public float steering;
    private Rigidbody rB;
    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        accelerator = Input.GetAxis("Vertical");
        steering = Input.GetAxis("Horizontal") * 45;
    }

    void FixedUpdate()
    {
        foreach ( WheelScript wheel in wheels)
        {
            
            if (wheel.steeringWheel)
            {
                wheel.transform.localRotation = Quaternion.Euler(steering + -90,-90,-90); 
            }

            
            RaycastHit hit;
            Physics.Raycast(wheel.transform.position, wheel.transform.TransformDirection(Vector3.down), out hit,maxDistance);
            if (hit.collider != null && hit.collider.gameObject != this)
            { 
                //Suspension
                rB.AddForceAtPosition(wheel.transform.up * forceMultiplier * springCurve.Evaluate(hit.distance), wheel.transform.position);
                wheel.wheelModel.transform.position = hit.point + Vector3.up *0.4f;

                if (wheel.driveWheel)
                {
                  rB.AddForceAtPosition(wheel.transform.forward * accelerator * (baseEngineTorque/DriveWheels()),wheel.transform.position);  
                }
                
                Vector3 localVelocity = wheel.transform.InverseTransformDirection(rB.velocity);

                rB.AddForceAtPosition (wheel.transform.TransformDirection(new Vector3(-localVelocity.x *0.8f,0,0))  * 0.8f ,wheel.transform.position);
            }
            


        }
    }
    
    public int DriveWheels()
    {
        int i = 0;
        foreach (WheelScript wheel in wheels)
        {
            if (wheel.driveWheel)
            {
                i++;
            }
        }

        return i;
    }
}
