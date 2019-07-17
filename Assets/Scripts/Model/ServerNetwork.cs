﻿using System.Collections;
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
    public PlanetManager planet;
    public Player serverReference;

    private void Awake()
    {

        _view = GetComponent<PhotonView>();

        if (!instance)
        {
            if (_view.IsMine)
            {
                _view.RPC("SetReferenceToSelf", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
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
            _view.RPC("AddPlayer", serverReference, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void AddPlayer(Player p)
    {
        if (!_view.IsMine) 
            return;
        var newPlayer = PhotonNetwork.Instantiate("Car",
                        new Vector3(Random.Range(0, 3),
                        Random.Range(0, 3),
                        Random.Range(0, 3)),
                        Quaternion.identity).GetComponent<CarController>();
        players.Add(p, newPlayer); //Lo añado al diccionario enlazando el jugador con su Hero
        foreach (var item in players)
        {
            Debug.Log(item);
        }
    }

    //De ahora en mas son comandos para mover a los players

    [PunRPC]
    void RequestMove(Vector3 dir, Player p)
    {
        if (!_view.IsMine)
            return;
        if (players.ContainsKey(p))
            players[p].Move(dir);
    }

    public void PlayerRequestMove(Vector3 dir, Player p)
    {
        _view.RPC("RequestMove", serverReference,dir, p);
    }
}
