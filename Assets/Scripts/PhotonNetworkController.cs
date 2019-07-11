using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonNetworkController : MonoBehaviourPunCallbacks
{
    private bool isHost;

    void Start()
    {
        DontDestroyOnLoad(this); //Never destroy
    }

    public void OnHostServerButton()
    {
        isHost = true;
        PhotonNetwork.ConnectUsingSettings(); //Try to connect
    }

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

    public override void OnJoinedLobby()
    {
        if (isHost)
        {
            PhotonNetwork.CreateRoom("MainRoom", new RoomOptions() { MaxPlayers = 8 });
        }
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        if (isHost)
        {
            PhotonNetwork.Instantiate("ServerNetwork", Vector3.zero, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        
    }
}
