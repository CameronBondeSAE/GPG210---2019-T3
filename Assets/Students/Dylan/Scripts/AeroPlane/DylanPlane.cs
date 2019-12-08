using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanPlane : Possessable
{
    public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 200.0f;

    public Rigidbody rb;

    public DylanWing dylanWing;

    public List<GameObject> planeEngines;

    public float roll;
    public float pitch;
    public float yaw;
    public float acceleration;

    public Quaternion AddRot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dylanWing = GetComponent<DylanWing>();
    }

    void FixedUpdate()
    {
        Quaternion AddRot = Quaternion.identity;

        //roll = Input.GetAxis("Roll") * (Time.deltaTime * RotationSpeed);
        //pitch = Input.GetAxis("Vertical") * (Time.deltaTime * RotationSpeed);
        //yaw = Input.GetAxis("Horizontal") * (Time.deltaTime * RotationSpeed);
        //acceleration = Input.GetAxis("Jump") * AmbientSpeed;

        dylanWing.RotatePlane(pitch, yaw, roll);
        //dylanWing.RotatePlanePitch(pitch);
        //dylanWing.RotatePlaneYaw(yaw);
        //dylanWing.RotatePlaneRoll(roll);
        dylanWing.MoveForward(acceleration);
        
        //Vector3 AddPos = Vector3.forward;
        //AddPos = rb.rotation * AddPos;
        //rb.velocity = AddPos * (Time.deltaTime * AmbientSpeed);
    }


    public override void RightStickAxis(Vector2 value)
    {
        pitch = value.x * (Time.deltaTime * RotationSpeed);
        yaw = value.y * (Time.deltaTime * RotationSpeed);
    }

    public override void LeftStickAxis(Vector2 value)
    {
        roll = value.x * (Time.deltaTime * RotationSpeed);

        base.LeftStickAxis(value);
    }

    public override void RightTrigger(float value)
    {
        acceleration = value;
    }


}
