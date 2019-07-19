using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviourPun
{
    private const int CAMERA_DISTANCE = 10;
    public CarController player;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    // Update is called once per frame
    public void Move()
    {
        if (player)
        {
            transform.position = player.transform.position + player.transform.up * CAMERA_DISTANCE;
            transform.LookAt(player.transform);
        }
    }
}
