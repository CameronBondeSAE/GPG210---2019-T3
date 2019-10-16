using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Vector3 force;
    public Vector3 offset;

    public AnimationCurve springStrengthCurve;

    public Transform thrusterPoint;
    public float maxDistance;
    Rigidbody rb;
    Transform t;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(t.position, Vector3.down, out hitInfo, maxDistance))
        {
//            transform.TransformDirection(Vector3.down)
            
//          position is a WORLD position. So a local offset, requires you to add the current position
            rb.AddForceAtPosition(force * (maxDistance - hitInfo.distance), t.position + offset);
            rb.AddForceAtPosition(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")),
                t.position + offset);
        }
    }

}
