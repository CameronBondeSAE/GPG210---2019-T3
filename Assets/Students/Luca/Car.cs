using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Car : MonoBehaviour
{
    public List<Wheel> steeringWheels;
    public List<Wheel> driveWheels;

    public float motorStrength;
    public Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if(steeringWheels == null)
            steeringWheels = new List<Wheel>();
        if(driveWheels == null)
            driveWheels = new List<Wheel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            float wheelForce = motorStrength / driveWheels.Count;
            foreach (var wheel in driveWheels)
            {
                wheel.ApplyForce(wheelForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (var wheel in steeringWheels)
            {
                wheel.inputTurnLeft = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            foreach (var wheel in steeringWheels)
            {
                wheel.inputTurnLeft = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (var wheel in steeringWheels)
            {
                wheel.inputTurnRight = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            foreach (var wheel in steeringWheels)
            {
                wheel.inputTurnRight = false;
            }
        }
    }

    public void AddForce(Vector3 force, Vector3 offset)
    {
        
    }
}
