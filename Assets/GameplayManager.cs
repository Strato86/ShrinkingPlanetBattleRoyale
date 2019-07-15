using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameplayManager : MonoBehaviour
{
    public GameObject playerPrefab;
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 25.25f,0), Quaternion.identity);
        if (PhotonNetworkController.instance.isServer) 
        {
            PhotonNetwork.Instantiate("ServerNetwork", Vector3.zero, Quaternion.identity);
         }
    }
}
