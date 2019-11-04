using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanPlane : MonoBehaviour
{
    public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 200.0f;

    private Rigidbody rb;

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
        AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        GetComponent<Rigidbody>().rotation *= AddRot;
        Vector3 AddPos = Vector3.forward;
        AddPos = rb.rotation * AddPos;
        GetComponent<Rigidbody>().velocity = AddPos * (Time.deltaTime * AmbientSpeed);
    }
}
