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
        players.Add(p, newPlayer); //Lo añado al diccionario enlazando el jugador con su CarController

        //TODO: Camera
        if (players.Count > 1)
        {
            GameMasterManager.instance.StartGame();
        }

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

    [PunRPC]
    void AsignCamera(Player p)
    {
        if (!_view.IsMine)
            return;
        if (players.ContainsKey(p))
            players[p].isTaken = true;
    }

    [PunRPC]
    void DestroyPlayer(Player p)
    {
        Debug.Log("Entro a destruir Player");
        if (!_view.IsMine)
        {
            Debug.Log("I dont control this bro!");
            return;
        }
        if (players.ContainsKey(p))
        {
            var player = players[p];
            players.Remove(p);
            PhotonNetwork.Destroy(player.gameObject);
            Debug.Log("Destroy Player: " + p.UserId);
        }
        else
        {
            Debug.Log(p + "ya no esta en el dic");
            foreach (var pl in players)
            {
                Debug.Log(pl);
            }
        }
    }

    public void PlayerRequestMove(Vector3 dir, Player p)
    {
        _view.RPC("RequestMove", serverReference,dir, p);
    }

    public void PlayerAsignCamera(Player p)
    {
        _view.RPC("AsignCamera", serverReference, p);
    }

    public void RequestDestroyPlayer(Player p)
    {
        Debug.Log("Request Destroy Player");
        _view.RPC("DestroyPlayer", serverReference, p);
    }

    public void CraterRequestInstantiate(Vector3 pos)
    {
        if (!_view.IsMine)
            return;

        var dir = (pos - Vector3.zero).normalized;
        var craterGO = PhotonNetwork.Instantiate("Crater", pos, Quaternion.LookRotation(dir));
        craterGO.transform.SetParent(planet.transform);
    }

    public void RequestInstantiateComet(Vector3 pos)
    {
        if (!photonView.IsMine)
            return;

        var dir = (Vector3.zero - pos).normalized;
        PhotonNetwork.Instantiate("Comet", pos, Quaternion.LookRotation(dir));

    }
}
