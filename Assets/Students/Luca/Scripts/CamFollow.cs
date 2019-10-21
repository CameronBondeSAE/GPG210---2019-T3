using System.Collections;
using System.Collections.Generic;
using Students.Luca;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 desiredDistance = new Vector3(0,5,-10);
    public float maxFollowSpeed = 30;
    public float maxFollowSpeedDistance = 10; // When cam is this dist away from the desired location, it travels at full speed
    public float rotationSpeed = 30;

    private Vector3 desiredPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDesiredPosition();

        MoveToDesiredPosition();

        SmoothLookAtTarget();
        
        Debug.DrawLine(desiredPosition,followObject.transform.position,Color.blue);
        
    }

    private void CalculateDesiredPosition()
    {
        Vector3 forwardVecNoY = followObject.transform.TransformVector(desiredDistance);
        forwardVecNoY.y = followObject.transform.position.y + desiredDistance.y;
        
        desiredPosition = followObject.transform.position + forwardVecNoY;
    }

    private void MoveToDesiredPosition()
    {
        float distanceToDesiredPos = Vector3.Distance(transform.position, desiredPosition);
        if (distanceToDesiredPos > 3)
        {
            float followSpeed = Mathf.Clamp((distanceToDesiredPos/maxFollowSpeedDistance)*maxFollowSpeed, 1, maxFollowSpeed);
            Debug.Log(followSpeed);
            Vector3 newPos = Vector3.MoveTowards(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }

    private void SmoothLookAtTarget()
    {
        Vector3 targetDir = followObject.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
