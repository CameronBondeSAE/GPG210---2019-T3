using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    
    public List<PlayerInfo> playerInfos;
    
    public LayerMask defaultLayersForCullingMasks;

    public GameObject playerCharacterPrefab;
    
    public Transform playerSpawnLocation;

    void Start()
    {
        //playerInputManager = FindObjectOfType<PlayerInputManager>();
        //playerInputManager.playerJoinedEvent.AddListener(OnPlayerJoin);
        //playerInputManager.playerLeftEvent.AddListener(OnPLayerLeave);
        playerInputManager.EnableJoining();
    }
    public void OnPlayerJoined(PlayerInput p)
    {
        Debug.Log("got here..");
        PlayerInfo pI = GetPlayerInfo(p);
        //playerInfos.Add(pI);
        SetUpCameras(pI);
        playerInputManager.EnableJoining();
        pI.controller.possessable = pI.playerCharacterPossessable;
    }

    public void OnPLayerLeft(PlayerInput p)
    {
        foreach (PlayerInfo pI in playerInfos)
        {
            if (pI.playerInput == p)
            {
                playerInfos.Remove(pI);
            }
        }
    }

    public PlayerInfo GetPlayerInfo(PlayerInput p)
    {
        PlayerInfo pI;
        pI.playerInput = p;
        pI.virtualCameraLayer = p.playerIndex + 9;
        pI.controller = p.GetComponent<Controller>();
        pI.playerCharacter = SpawnPlayerCharacterPrefab(p.playerIndex);
        pI.realCamera = p.GetComponent<Camera>();
        pI.virtualCamera = pI.playerCharacter.GetComponentInChildren<CinemachineVirtualCamera>();
        pI.playerCharacterPossessable = pI.playerCharacter.GetComponent<Possessable>();
        pI.playerVehicleInteraction = p.GetComponent<PlayerVehicleInteraction>();
        pI.playerVehicleInteraction.currentPossessed = pI.playerCharacter.GetComponent<Possessable>();
        pI.playerVehicleInteraction.playerCharacterPossessable = pI.playerCharacterPossessable;
        pI.playerVehicleInteraction.playerCharacterGameObjectObject = pI.playerCharacter;
        return pI;
    }

    public void SetCameraLayerMask(Camera c,int i)
    {
        // just sets up the culling mask by setting the default stuff, and the layer for the virtual cameras for this player, but not the layers for the other players.
        int layerMask = 1 << i;
        c.cullingMask = defaultLayersForCullingMasks | layerMask;
    }

    public void SetUpCameras(PlayerInfo pI)
    {
        SetCameraLayerMask(pI.realCamera, pI.virtualCameraLayer);
        pI.virtualCamera.gameObject.layer = pI.virtualCameraLayer;
    }



    public GameObject SpawnPlayerCharacterPrefab(int i)
    {
        return Instantiate(playerCharacterPrefab, playerSpawnLocation.position + new Vector3(i,0,0), playerSpawnLocation.rotation);
    }
}

public struct PlayerInfo
{
    public Possessable playerCharacterPossessable;
    public Camera realCamera;
    public Controller controller;
    public int virtualCameraLayer;
    public GameObject playerCharacter;
    public PlayerInput playerInput;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerVehicleInteraction playerVehicleInteraction;
}
