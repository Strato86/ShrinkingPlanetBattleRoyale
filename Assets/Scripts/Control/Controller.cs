using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Controller : MonoBehaviourPun
{
    private PhotonView _view;
    // Start is called before the first frame update
    void Start()
    {
        _view = GetComponent<PhotonView>();
            //TODO: Asign camera
    }

    void Update()
    {
        if (!_view.IsMine)
            return;
        
        ServerNetwork.instance.PlayerRequestMove(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0), PhotonNetwork.LocalPlayer);
    }


}
