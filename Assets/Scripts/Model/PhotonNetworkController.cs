using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonNetworkController : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkController instance { get; private set; } //Singleton

    private bool isHost;
    private int _playerCount;

    public Text playerCount;
    public byte maxPlayersPerRoom = 8;
    public byte minPlayersToStartGame = 1;

    public bool isServer;

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

    void Start()
    {
        DontDestroyOnLoad(this); //Never destroy
        playerCount.enabled = false;
    }
    /// <summary>
    /// Host Button Pressed
    /// </summary>
    public void OnHostServerButton()
    {
        isHost = true;
        PhotonNetwork.ConnectUsingSettings(); //Try to connect
    }

    /// <summary>
    /// Join Button Pressed
    /// </summary>
    public void OnJoinServerButton()
    {
        PhotonNetwork.ConnectUsingSettings(); //Try to connect
    }

    /// <summary>
    /// On connected to master callback
    /// </summary>
    public override void OnConnectedToMaster()
    {
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
        /*if (isHost)
        {
            PhotonNetwork.Instantiate("ServerNetwork", Vector3.zero, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity);
        }*/
        isServer = isHost;
        playerCount.enabled = true;
        if(_playerCount >= minPlayersToStartGame - 1)
        {
            SceneManager.LoadScene(1);
        }
    }

    /// <summary>
    /// On Join room fail callback
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.Disconnect();
    }

    /// <summary>
    /// On Create room callback
    /// </summary>
    public override void OnCreatedRoom()
    {
        
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
            _playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if(playerCount != null)
        playerCount.text = "Players: " + _playerCount + "/" + maxPlayersPerRoom;
    }
}
