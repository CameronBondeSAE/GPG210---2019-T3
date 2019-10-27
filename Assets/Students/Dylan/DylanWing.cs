using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanWing : MonoBehaviour
{
    /*
    public float thrusterForceMultiplier;

    public DylanCar mainBody;

    public float maxDistance = 3;

    public float lateralFriction = 30;

    public AnimationCurve springStrength;
    public float springLength = 1.5f;

    private void Start()
    {
        mainBody = FindObjectOfType<DylanCar>();
    }

    private void FixedUpdate()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, springLength);

        Vector3 localVelocity = transform.InverseTransformDirection(mainBody.rb.velocity);
        onGround = (hitInfo.collider != null);

        if (onGround)
        {
            Vector3 direction = new Vector3((-localVelocity.x * lateralFriction) * springStrength.Evaluate(Mathf.Abs(localVelocity.x)), 0, 0);
            mainBody.rb.AddForceAtPosition((transform.up * thrusterForceMultiplier) * (springLength - hitInfo.distance), transform.position);
            wheelModel.transform.position = hitInfo.point + Vector3.up * 0.5f;
        }


    }*/
}
