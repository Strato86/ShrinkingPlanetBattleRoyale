using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCamera : MonoBehaviour
{
    private const int CAMERA_DISTANCE = 10;
    private bool _isAsigned;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.enabled = false;
    }
    // Update is called once per frame
    public void Update()
    {
        if (!_isAsigned)
        {
            if (ServerNetwork.instance != null)
            {
                if(ServerNetwork.instance.players.ContainsKey(PhotonNetwork.LocalPlayer))
                {
                    var p = ServerNetwork.instance.players[PhotonNetwork.LocalPlayer];
                    _isAsigned = true;
                    cam.enabled = true;
                    transform.position = p.transform.position + p.transform.up * CAMERA_DISTANCE;
                    transform.LookAt(p.transform);
                    transform.SetParent(p.transform);
                }
            }
            
        }
    }
}
