using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanPlane : Possessable
{
    public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 200.0f;

    private Rigidbody rb;

    private float roll;
    private float pitch;
    private float yaw;
    private float acceleration;

    private Quaternion AddRot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion AddRot = Quaternion.identity;

        //roll = Input.GetAxis("Roll") * (Time.deltaTime * RotationSpeed);
        //pitch = Input.GetAxis("Vertical") * (Time.deltaTime * RotationSpeed);
        //yaw = Input.GetAxis("Horizontal") * (Time.deltaTime * RotationSpeed);
        //acceleration = Input.GetAxis("Jump") * (Time.deltaTime * AmbientSpeed);

        RotatePlane(pitch, yaw, roll);
        MoveForward(acceleration);
        
        //Vector3 AddPos = Vector3.forward;
        //AddPos = rb.rotation * AddPos;
        //rb.velocity = AddPos * (Time.deltaTime * AmbientSpeed);
    }

    void RotatePlane(float pitch, float yaw, float roll)
    {
        AddRot.eulerAngles = new Vector3(pitch, yaw, roll);
        rb.rotation *= AddRot;
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

    void MoveForward(float acceleration)
    {
        Vector3 direction = Vector3.forward;
        direction = rb.rotation * direction;
        //rb.AddForce(transform.TransformDirection(direction) * acceleration,ForceMode.Acceleration);
        rb.velocity = direction * acceleration;
    }

}
