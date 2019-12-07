using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEngine.InputSystem;

public class PlayerManager : SerializedMonoBehaviour
{
    
    /// <summary>
    /// This is the player manager, It handles taking info from the playerInputManager
    /// (which spawns players, sets their controls and even sets up splitscreen) and it also handles setting up other game player information
    /// Like the players virtual camera, culling layer,player vehicle interaction etc.
    /// It also keeps a list of player info for currently joined players. and fires events when players join and leave.
    /// </summary>
    public PlayerInputManager playerInputManager;
    
    [SerializeField]
    public List<PlayerInfo> playerInfos;
    
    public LayerMask defaultLayersForCullingMasks;

    public GameObject playerCharacterPrefab;
    
    public Transform playerSpawnLocation;

    #region Events

    public delegate void PlayerDel(PlayerInfo playerInfo);
    public event PlayerDel OnNewPlayerJoinedGame;
    public event PlayerDel OnPlayerLeftGame;

    protected void NotifyNewPlayerJoinedGame(PlayerInfo playerInfo)
    {
        OnNewPlayerJoinedGame?.Invoke(playerInfo);
    }
    
    protected void NotifyPlayerLeftGame(PlayerInfo playerInfo)
    {
        OnPlayerLeftGame?.Invoke(playerInfo);
    }
    
    #endregion
    
    void Start()
    {
        playerInfos = new List<PlayerInfo>();
        playerInputManager.EnableJoining();
    }
    public void OnPlayerJoined(PlayerInput p)
    {
        PlayerInfo pI = GetPlayerInfo(p);
        playerInfos.Add(pI);
        SetUpCameras(pI);
        playerInputManager.EnableJoining();
        pI.controller.possessable = pI.playerCharacterPossessable;
        
        NotifyNewPlayerJoinedGame(pI);
    }

    public void OnPLayerLeft(PlayerInput p)
    {
        foreach (PlayerInfo pI in playerInfos)
        {
            if (pI.playerInput == p)
            {
                playerInfos.Remove(pI);
                NotifyPlayerLeftGame(pI);
                break;
            }
        }
    }

    public PlayerInfo GetPlayerInfo(PlayerInput p)
    {
        PlayerInfo pI = new PlayerInfo();
        pI.playerInput = p;
        pI.virtualCameraLayer = p.playerIndex + 9;
        pI.controller = p.GetComponent<Controller>();
        pI.controller.playerInfo = pI;
        pI.playerCharacter = SpawnPlayerCharacterPrefab(p.playerIndex);
        pI.realCamera = p.GetComponent<Camera>();
        pI.virtualCamera = pI.playerCharacter.GetComponentInChildren<CinemachineVirtualCamera>();
        pI.playerCharacterPossessable = pI.playerCharacter.GetComponent<Possessable>();
        pI.playerVehicleInteraction = p.GetComponent<PlayerVehicleInteraction>();
        pI.playerVehicleInteraction.playerInfo = pI;
        pI.playerVehicleInteraction.currentPossessed = pI.playerCharacter.GetComponent<Possessable>();
        pI.playerVehicleInteraction.playerCharacterPossessable = pI.playerCharacterPossessable;
        pI.playerVehicleInteraction.playerCharacterGameObjectObject = pI.playerCharacter;

        Debug.Log("Got player info :" + p.playerIndex);
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

public class PlayerInfo
{
    public string name;
    public int score;
    public Possessable playerCharacterPossessable;
    public Camera realCamera;
    public Controller controller;
    public int virtualCameraLayer;
    public GameObject playerCharacter;
    public PlayerInput playerInput;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerVehicleInteraction playerVehicleInteraction;
}
