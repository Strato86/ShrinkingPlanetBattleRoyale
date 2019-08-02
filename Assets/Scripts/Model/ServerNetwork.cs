using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class ServerNetwork : MonoBehaviourPun
{
    public static ServerNetwork instance { get; private set; }

    
    public Dictionary<Player, CarController> players = new Dictionary<Player, CarController>();
    public Dictionary<Player, UIController> uis = new Dictionary<Player, UIController>();

    public PlanetManager planet;
    public Player serverReference;

    public int secondsToStart = 3;

    private List<CarController> _losers;

    private void Awake()
    {
        _losers = new List<CarController>();

        if (!instance)
        {
            if (photonView.IsMine)
            {
                photonView.RPC("SetReferenceToSelf", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
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
            photonView.RPC("AddPlayer", serverReference, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    public void AddPlayer(Player p)
    {
        if (!photonView.IsMine)
            return;

        var position = new Vector3();
        var rotation = new Quaternion();

        foreach (var sp in planet.carSpawnPoints)
        {
            if (!sp.isTaken)
            {
                position = sp.transform.position;
                rotation = sp.transform.rotation;
                sp.isTaken = true;
                break;
            }
        }
        var newPlayer = PhotonNetwork.Instantiate("Car",
                        position,
                        rotation).GetComponent<CarController>();

        players.Add(p, newPlayer); //Lo añado al diccionario enlazando el jugador con su CarController
    }

    //De ahora en mas son comandos para mover a los players

    [PunRPC]
    void RequestMove(Vector3 dir, Player p)
    {
        if (!photonView.IsMine)
            return;
        if (!GameMasterManager.instance.gameActive)
            return;
        if (players.ContainsKey(p))
            players[p].Move(dir);
    }

    [PunRPC]
    void SetMoving(float horizontal, Player p)
    {
        if (!photonView.IsMine)
            return;
        if (!GameMasterManager.instance.gameActive)
            return;
        if (players.ContainsKey(p))
        {
            players[p].SetAnimation(horizontal);
        }
    }

    [PunRPC]
    void AsignCamera(Player p)
    {
        if (!photonView.IsMine)
            return;
        if (players.ContainsKey(p))
            players[p].isTaken = true;
    }

    [PunRPC]
    void AsignPlayerID(string id, Player p)
    {
        Debug.Log("Asigno ID: " + id);
        if (!photonView.IsMine)
        {
            Debug.Log("No puedo asignar id porque no es mio");
            return;
        }
        if (players.ContainsKey(p))
        {
            players[p].playerID = id;
        }
        else
        {
            Debug.Log("No esta en el diccionario ???");
        }
    }

    public bool StartGameFromServer()
    {
        if (players.Count >= PhotonNetworkController.instance.minPlayersToStartGame)
        {
            StartCoroutine(CountDown());
            EventManager.DispatchEvent(GameEvent.START_GAME);
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator CountDown()
    {
        for (int i = secondsToStart; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
        }
        GameMasterManager.instance.StartGame();
    }

    //Para el movimiento del player
    public void PlayerRequestMove(Vector3 dir, Player p)
    {
        photonView.RPC("RequestMove", serverReference,dir, p);
        photonView.RPC("SetMoving", RpcTarget.All, dir.x, p); //RPC para sincronizar en todos mi animación de movimiento.
    }

    //Asigno la camara al player, para poner en true la variable isTaken del carController
    public void PlayerAsignCamera(Player p)
    {
        photonView.RPC("AsignCamera", serverReference, p);
    }

    //Asigno el ID del player al car controller, para poder asignar la camara bien
    public void PlayerAsignID(string id, Player p)
    {
        photonView.RPC("AsignPlayerID", serverReference, id, p);
    }

    //Crea un crater, setea el parent al planeta y lo sincroniza a los otros users
    public void CraterRequestInstantiate(Vector3 pos)
    {
        if (!photonView.IsMine)
            return;

        var dir = (pos - Vector3.zero).normalized;
        var craterGO = PhotonNetwork.Instantiate("Crater", pos, Quaternion.LookRotation(dir));
        craterGO.transform.SetParent(planet.transform);
    }

    //Instancia un cometa desde el server en una posicion dada y mirando hacia el planeta
    public void RequestInstantiateComet(Vector3 pos)
    {
        if (!photonView.IsMine)
            return;

        var dir = (Vector3.zero - pos).normalized;
        PhotonNetwork.Instantiate("Comet", pos, Quaternion.LookRotation(dir));

    }

    //Destroye le cometa desde el server
    public void RequestDestroyComet(GameObject go)
    {
        if (!photonView.IsMine)
            return;
        PhotonNetwork.Destroy(go);
    }

    public void ParticleRequestInstantiate(string particleName, Transform pos)
    {
        if (!photonView.IsMine)
            return;

        PhotonNetwork.Instantiate(particleName, pos.position, pos.rotation);
    }

    public void SetLoser(CarController c)
    {
        if (!photonView.IsMine)
            return;
        if (!_losers.Contains(c))
        {
            _losers.Add(c);
        }

        if(_losers.Count + 1 >= players.Count)
        {
            Player winner = null;
            foreach (var p in players)
            {
                if (!_losers.Contains(p.Value))
                    winner = p.Key;
            }
            if (winner != null)
                EventManager.DispatchEvent(GameEvent.END_GAME, winner.NickName);
            else 
                EventManager.DispatchEvent(GameEvent.END_GAME, "Nobody");
            GameMasterManager.instance.EndGame();
            StartCoroutine(EndGame());
            _losers.Clear();
            players.Clear();
        }

        IEnumerator EndGame()
        {
            yield return new WaitForSeconds(10);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
