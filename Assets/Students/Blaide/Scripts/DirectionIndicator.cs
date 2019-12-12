using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Students.Luca.Scripts.Checkpoints;
using UnityEditor;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    public Controller controller;
    public PlayerInfo playerInfo;
    public Checkpoint closestActiveCheckpoint;
    public GameObject virtualCameraObject;
    public Vector3 positionOffset;
    public CheckPointChicken checkPointChicken;
    public List<Checkpoint> activeCheckpoints;
    public GameObject meshObject;
    // Start is called before the first frame update
    void Start()
    {
        controller = transform.root.GetComponent<Controller>();
        playerInfo = controller.playerInfo;
        gameObject.layer = playerInfo.virtualCameraLayer;
        virtualCameraObject = playerInfo.virtualCamera.gameObject;
        checkPointChicken = FindObjectOfType<CheckPointChicken>();
        checkPointChicken.OnTargetCheckpointChanged += UpdateTarget;
        meshObject = transform.GetChild(0).gameObject;
        meshObject.layer = playerInfo.virtualCameraLayer;
        if(closestActiveCheckpoint == null)
        {
            closestActiveCheckpoint = checkPointChicken.startCheckpoint;
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*foreach (Checkpoint c in activeCheckpoints)
        {
            if (closestActiveCheckpoint != null && c != null)
            {
                if (Vector3.Distance(c.transform.position, this.transform.position) < (Vector3.Distance(closestActiveCheckpoint.transform.position, this.transform.position)))
                {
                    closestActiveCheckpoint = c;
                }
            }
            else if( c != null)
            {
                closestActiveCheckpoint = c;
            }
        }
        this.transform.LookAt( new Vector3(closestActiveCheckpoint.transform.position.x,transform.position.y,closestActiveCheckpoint.transform.position.z));*/
        this.transform.LookAt( new Vector3(checkPointChicken.currentCheckpoint.transform.position.x,transform.position.y,checkPointChicken.currentCheckpoint.transform.position.z));
    }

    public void UpdateTarget(CheckpointReachedPlayerData checkpointReachedPlayerData)
    {
        activeCheckpoints = closestActiveCheckpoint.nextCheckpoints;
        closestActiveCheckpoint = null;

    }
}

