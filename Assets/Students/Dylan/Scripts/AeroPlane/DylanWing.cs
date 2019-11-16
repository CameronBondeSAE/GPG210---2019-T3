using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanWing : MonoBehaviour
{
    public DylanPlane mainBody;

    private float roll;
    private float pitch;
    private float yaw;
    private float acceleration;

    public void Start()
    {
        roll = mainBody.roll;
        pitch = mainBody.pitch;
        yaw = mainBody.yaw;
        acceleration = mainBody.acceleration;
    }

    public void Update()
    {
        roll = mainBody.roll;
        pitch = mainBody.pitch;
        yaw = mainBody.yaw;
        acceleration = mainBody.acceleration;
    }

    public void RotatePlane(float pitch, float yaw, float roll)
    {
        mainBody.AddRot.eulerAngles = new Vector3(pitch, yaw, roll);
        mainBody.rb.rotation *= mainBody.AddRot;
        //mainBody.rb.rotation = Quaternion.Euler(pitch, yaw, roll);
    }

    public void RotatePlanePitch(float pitch)
    {
        mainBody.AddRot.eulerAngles = new Vector3(pitch, 0, 0);
        mainBody.rb.rotation *= mainBody.AddRot;
    }

    public void RotatePlaneYaw(float yaw)
    {
        mainBody.AddRot.eulerAngles = new Vector3(0, yaw, 0);
        mainBody.rb.rotation *= mainBody.AddRot;
    }

    public void RotatePlaneRoll(float roll)
    {
        mainBody.AddRot.eulerAngles = new Vector3(0, 0, roll);
        mainBody.rb.rotation *= mainBody.AddRot;
    }

    public void MoveForward(float acceleration)
    {
        /*
        Vector3 direction = Vector3.forward;
        direction = mainBody.rb.rotation * direction;
        //rb.AddForce(transform.TransformDirection(direction) * acceleration,ForceMode.Acceleration);
        mainBody.rb.velocity = direction * acceleration;
        */
        foreach (GameObject wheel in mainBody.planeEngines)
        {
            mainBody.rb.AddForceAtPosition(transform.TransformDirection(Vector3.forward) * acceleration * Time.deltaTime, wheel.transform.position);
            
        }

    }
    
}
