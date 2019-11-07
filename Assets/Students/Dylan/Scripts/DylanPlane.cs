using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanPlane : MonoBehaviour
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

        roll = 0f;
        pitch = 0f;
        yaw = 0f;
        acceleration = 0f;

        roll = Input.GetAxis("Roll") * (Time.deltaTime * RotationSpeed);
        pitch = Input.GetAxis("Pitch") * (Time.deltaTime * RotationSpeed);
        yaw = Input.GetAxis("Yaw") * (Time.deltaTime * RotationSpeed);
        acceleration = Input.GetAxis("Jump") * (Time.deltaTime * AmbientSpeed);

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

    void MoveForward(float acceleration)
    {
        Vector3 direction = Vector3.forward;
        direction = rb.rotation * direction;
        //rb.AddForce(transform.TransformDirection(direction) * acceleration,ForceMode.Acceleration);
        rb.velocity = direction * acceleration;
    }

}
