using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMainSystem : MonoBehaviour
{
    public float baseEngineTorque;
    public AnimationCurve springCurve;
    public List<Wheel> wheels;
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
        //steering = Input.GetAxis("Horizontal") * 60;
    }

    void FixedUpdate()
    {
        foreach ( Wheel wheel in wheels)
        {
            RaycastHit hit;
            
            Physics.Raycast(wheel.transform.position, wheel.transform.TransformDirection(Vector3.down), out hit,maxDistance);
                    
            if (hit.collider != null && hit.collider.gameObject != this)
            { 
                rB.AddForceAtPosition(wheel.transform.up * forceMultiplier * springCurve.Evaluate(hit.distance), wheel.transform.position);

                wheel.wheelModel.transform.position = hit.point + Vector3.up *0.4f;

                if (wheel.driveWheel)
                {
                  rB.AddForceAtPosition(wheel.transform.forward * accelerator * (baseEngineTorque/DriveWheels()),wheel.transform.position);  
                }
            }
            
            if (wheel.steeringWheel)
            {
                wheel.transform.rotation =  Quaternion.Euler(0,steering,0);
            }

        }
    }
    
    public int DriveWheels()
    {
        int i = 0;
        foreach (Wheel wheel in wheels)
        {
            if (wheel.driveWheel)
            {
                i++;
            }
        }

        return i;
    }
}
