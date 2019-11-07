using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DylanCameraController : MonoBehaviour
{
    public GameObject player;

    public Transform cameraPos;

    private void LateUpdate()
    {
        MoveToPosition(cameraPos);
        transform.LookAt(player.transform, Vector3.up);
    }

    void MoveToPosition(Transform pos)
    {

        transform.position = pos.transform.position;

    }
}
