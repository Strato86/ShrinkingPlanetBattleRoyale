using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class ServerNetwork : MonoBehaviourPun
{
    public static ServerNetwork instance { get; private set; }

    PhotonView _view;
    public Dictionary<Player, CarController> players = new Dictionary<Player, CarController>();
    public Player serverReference;

    private void Awake()
    {

        _view = GetComponent<PhotonView>();

        if (!instance)
        {
            if (_view.IsMine)
            {
                _view.RPC("SetReferenceToSelf", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer); //
            }
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    public void SetReferenceToSelf(Player p)
    {
        instance = this;
        serverReference = p;
        if (!PhotonNetwork.IsMasterClient)
        {
            _view.RPC("AddPlayer", serverReference, PhotonNetwork.LocalPlayer); //Call server to add player
        }
    }

    [PunRPC]
    public void AddPlayer(Player p)
    {
        if (!_view.IsMine)
            return;
        var newPlayer = PhotonNetwork.Instantiate("Player",
                        new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3)),
                        Quaternion.identity).GetComponent<CarController>();
        players.Add(p, newPlayer);
        foreach(var item in players)
        {
            Debug.Log(item);
        }
    }


}
