﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanCar : MonoBehaviour
{
    public List<GameObject> drivingWheels;
    public List<GameObject> turningWheels;

    private DylanThruster dylanThruster;

    public Transform centreOfMass;

    public float turningSpeed;
    public float speed;
    public float maxSpeed;

    public float maxRightAngle = -90;
    public float maxLeftAngle = 90;

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dylanThruster = FindObjectOfType<DylanThruster>();
        rb.centerOfMass = centreOfMass.localPosition;
    }

    private void FixedUpdate()
    {
        if (speed >= maxSpeed)
        {
            speed = maxSpeed;
        }

        if (dylanThruster.onGround)
        {
            if (Input.GetKey(left))
            {
                turningSpeed -= 1;
                TurnWheel(turningSpeed);
            }
            else if (Input.GetKey(right))
            {
                turningSpeed += 1;
                TurnWheel(turningSpeed);
            }
            else if (Input.GetKey(forward))
            {
                speed += 1f;
                dylanThruster.AddForwardThrust(speed);
            }
            else if (Input.GetKey(backward))
            {
                speed -= 1;
                dylanThruster.AddBackwardThrust(speed);
            }
            else
            {
                FixTireAngle();
                speed = 0;
            }
            
        }

        
    }
    void TurnWheel(float turnSpeed)
    {
        foreach (GameObject wheel in turningWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, turnSpeed, dylanThruster.defaultWheelRotation.z);
        }
    }

    public void FixTireAngle()
    {
        foreach (GameObject wheel in turningWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, 0, dylanThruster.defaultWheelRotation.z);
            turningSpeed = 0;
        }
    }

    /*
    void TurnBackWheels(float turnSpeed)
    {
        foreach (GameObject wheel in backWheels)
        {
            wheel.transform.localRotation = Quaternion.Euler(dylanThruster.defaultWheelRotation.x, turnSpeed, dylanThruster.defaultWheelRotation.z);
        }
    }
    */


}
