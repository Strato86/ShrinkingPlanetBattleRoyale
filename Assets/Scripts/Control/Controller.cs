using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Controller : MonoBehaviourPun
{

    private int curveMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {

        ServerNetwork.instance.PlayerAsignID(PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.LocalPlayer);
    }

    void Update()
    { 
        if (!photonView.IsMine)
            return;
        if (Input.GetKey(KeyCode.Space))
        {
            curveMultiplier = 2;
        }
        else
        {
            curveMultiplier = 1;
        }
        if(ServerNetwork.instance)
            ServerNetwork.instance.PlayerRequestMove(new Vector3(Input.GetAxis("Horizontal")* curveMultiplier, 1 + Input.GetAxis("Vertical"), 0), PhotonNetwork.LocalPlayer);
        else
            PhotonNetworkController.instance.DisconectUser();
    }
}
