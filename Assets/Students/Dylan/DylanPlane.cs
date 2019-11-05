using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanPlane : MonoBehaviour
{
    public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 200.0f;

    private Rigidbody rb;

    private Quaternion AddRot;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion AddRot = Quaternion.identity;

        float roll = 0;
        float pitch = 0;
        float yaw = 0;

        roll = Input.GetAxis("Roll") * (Time.deltaTime * RotationSpeed);
        pitch = Input.GetAxis("Pitch") * (Time.deltaTime * RotationSpeed);
        yaw = Input.GetAxis("Yaw") * (Time.deltaTime * RotationSpeed);

        RotatePlane(pitch, yaw, roll);
        
        Vector3 AddPos = Vector3.forward;
        AddPos = rb.rotation * AddPos;
        rb.velocity = AddPos * (Time.deltaTime * AmbientSpeed);
    }

    void RotatePlane(float pitch, float yaw, float roll)
    {
        AddRot.eulerAngles = new Vector3(pitch, yaw, roll);
        rb.rotation *= AddRot;
    }

}
