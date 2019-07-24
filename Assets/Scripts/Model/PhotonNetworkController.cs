using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonNetworkController : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkController instance { get; private set; } //Singleton

    public bool isHost;

    public Text playerCount;
    public Button playButton;
    public Transform cam;

    public byte maxPlayersPerRoom = 8;
    public byte minPlayersToStartGame = 1;

    private int _playerCount;

    private ServerNetwork _server;
    private GameObject _planetGO;
    //awake method
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    //if is server, just start it
    void Start()
    {
        DontDestroyOnLoad(this); //Never destroy
        playerCount.enabled = false;
        if (isHost)
        {
            CreateHostServer();
        }
    }
    /// <summary>
    /// Host Button Pressed
    /// </summary>
    public void CreateHostServer()
    {
        PhotonNetwork.ConnectUsingSettings(); //Try to connect
    }

    /// <summary>
    /// Join Button Pressed
    /// </summary>
    public void OnJoinServerButton()
    {
        playButton.interactable = false;
        PhotonNetwork.ConnectUsingSettings(); //Try to connect
    }

    /// <summary>
    /// On connected to master callback
    /// </summary>
    public override void OnConnectedToMaster()
    {
        playButton.gameObject.SetActive(false);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    /// <summary>
    /// On join lobby callback
    /// </summary>
    public override void OnJoinedLobby()
    {
        if (isHost)
        {
            PhotonNetwork.CreateRoom("MainRoom", new RoomOptions() { MaxPlayers = maxPlayersPerRoom });
            return;
        }
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// On Join Room callback
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (isHost)
        {
            Destroy(FindObjectOfType<PlayerCamera>());
            var serverGO = PhotonNetwork.Instantiate("ServerNetwork", Vector3.zero, Quaternion.identity);
            _server = serverGO.GetComponent<ServerNetwork>();
        }
        else
        {
            PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity);
        }
        playerCount.enabled = true;
    }

    /// <summary>
    /// On Join room fail callback
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.Disconnect();
        playButton.gameObject.SetActive(true);
        playButton.interactable = true;
    }

    /// <summary>
    /// On Create room callback
    /// </summary>
    public override void OnCreatedRoom()
    {
        PhotonNetwork.Instantiate("GameMasterManager", Vector3.zero, Quaternion.identity);
        _planetGO = PhotonNetwork.Instantiate("Planet", Vector3.zero, Quaternion.identity);
        Debug.Log("Room Created");
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
            _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if(playerCount != null)
            playerCount.text = "Players: " + (_playerCount - 1) + "/" + maxPlayersPerRoom;

        //Set reference to planet
        if(_server != null && _server.planet == null && _planetGO != null )
        {
            _server.planet = _planetGO.GetComponent<PlanetManager>();
        }
    }
}
